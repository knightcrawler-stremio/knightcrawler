from concurrent.futures import ThreadPoolExecutor, as_completed
from typing import Any, List

import Levenshtein
import PTN
import regex
from pydantic import BaseModel, validator

from .fetch import check_fetch
from .models import BaseRankingModel, ParsedData, SettingsModel
from .patterns import parse_extras
from .ranker import get_rank


class Torrent(BaseModel):
    """
    Represents a torrent with metadata parsed from its title and additional computed properties.

    Attributes:
        `raw_title` (str): The original title of the torrent.
        `infohash` (str): The SHA-1 hash identifier of the torrent.
        `parsed_data` (ParsedData): Metadata extracted from the torrent title including PTN parsing and additional extras.
        `fetch` (bool): Indicates whether the torrent meets the criteria for fetching based on user settings.
        `rank` (int): The computed ranking score of the torrent based on user-defined preferences.
        `lev_ratio` (float): The Levenshtein ratio comparing the parsed title and the raw title for similarity.

    Methods:
        __eq__: Determines equality based on the infohash of the torrent, allowing for easy comparison.
    """

    raw_title: str
    infohash: str
    parsed_data: ParsedData
    fetch: bool = False
    rank: int = 0
    lev_ratio: float = 0.0

    @validator("raw_title", "infohash")
    def validate_strings(cls, v):
        """Ensures raw_title and infohash are strings."""
        if not v:
            raise ValueError("Value cannot be empty.")
        if not isinstance(v, str):
            raise ValueError("Value must be a string.")
        return v

    @validator("infohash")
    def validate_infohash(cls, v):
        """Validates infohash length and SHA-1 format."""
        if len(v) != 40 or not regex.match(r"^[a-fA-F0-9]{40}$", v):
            raise ValueError("Infohash must be a 40-character SHA-1 hash.")
        return v

    def __eq__(self, other: object) -> bool:
        """Compares Torrent objects based on their infohash."""
        if not isinstance(other, Torrent):
            return False
        return self.infohash == other.infohash


class RTN:
    """
    RTN (Rank Torrent Name) class for parsing and ranking torrent titles based on user preferences.

    Attributes:
        `settings` (SettingsModel): The settings model with user preferences for parsing and ranking torrents.
        `ranking_model` (BaseRankingModel): The model defining the ranking logic and score computation.

    Methods:
        `rank`: Parses a torrent title, computes its rank, and returns a Torrent object with metadata and ranking.
    """

    def __init__(self, settings: SettingsModel, ranking_model: BaseRankingModel):
        """Initializes the RTN class with user settings and a ranking model."""
        if not settings or not ranking_model:
            raise ValueError("Both settings and a ranking model must be provided.")
        if not isinstance(settings, SettingsModel):
            raise TypeError("The settings must be an instance of SettingsModel.")
        if not isinstance(ranking_model, BaseRankingModel):
            raise TypeError("The ranking model must be an instance of BaseRankingModel.")

        self.settings = settings
        self.ranking_model = ranking_model

    def rank(self, raw_title: str, infohash: str) -> Torrent:
        """Parses a torrent title, computes its rank, and returns a Torrent object."""
        if not raw_title or not infohash:
            raise ValueError("Both the title and infohash must be provided.")
        if not isinstance(raw_title, str) or not isinstance(infohash, str):
            raise TypeError("The title and infohash must be strings.")
        if len(infohash) != 40:
            raise ValueError("The infohash must be a valid SHA-1 hash and 40 characters in length.")

        parsed_data = parse(raw_title)
        if not parsed_data:
            raise ValueError(f"Failed to parse the title: {raw_title}")
        return Torrent(
            raw_title=raw_title,
            infohash=infohash,
            parsed_data=parsed_data,
            fetch=check_fetch(parsed_data, self.settings),
            rank=get_rank(parsed_data, self.settings, self.ranking_model),
            lev_ratio=Levenshtein.ratio(parsed_data.parsed_title.lower(), raw_title.lower()),
        )


def parse(raw_title: str) -> ParsedData:
    """
    Parses a torrent title using PTN and enriches it with additional metadata extracted from patterns.

    Args:
        raw_title (str): The original torrent title to parse.

    Returns:
        ParsedData: A data model containing the parsed metadata from the torrent title.
    """
    if not raw_title or not isinstance(raw_title, str):
        raise TypeError("The input title must be a non-empty string.")

    parsed_dict: dict[str, Any] = PTN.parse(raw_title, coherent_types=True)  # Imagine this returns a dict
    extras: dict[str, Any] = parse_extras(raw_title)  # Returns additional fields as a dict
    full_data = {**parsed_dict, **extras}  # Merge PTN parsed data with extras
    full_data["raw_title"] = raw_title  # Add the raw title to the data
    full_data["parsed_title"] = parsed_dict.get("title")  # Add the parsed title to the data
    return ParsedData(**full_data)


def parse_chunk(chunk: List[str]) -> List[ParsedData]:
    """Parses a chunk of torrent titles."""
    return [parse(title) for title in chunk]


def batch_parse(titles: List[str], chunk_size: int = 50) -> List[ParsedData]:
    """
    Parses a list of torrent titles in batches for improved performance.

    Args:
        titles (List[str]): A list of torrent titles to parse.
        chunk_size (int): The number of titles to process in each batch.

    Returns:
        List[ParsedData]: A list of ParsedData objects for each title.
    """
    chunks = [titles[i : i + chunk_size] for i in range(0, len(titles), chunk_size)]
    parsed_data = []
    with ThreadPoolExecutor() as executor:
        future_to_chunk = {executor.submit(parse_chunk, chunk): chunk for chunk in chunks}
        for future in as_completed(future_to_chunk):
            chunk_result = future.result()
            parsed_data.extend(chunk_result)
    return parsed_data


def title_match(correct_title: str, raw_title: str, threshold: float = 0.9) -> bool:
    """
    Compares two titles using the Levenshtein ratio to determine similarity.

    Args:
        correct_title (str): The reference title to compare against.
        raw_title (str): The title to compare with the reference title.
        threshold (float): The similarity threshold to consider the titles as matching.

    Returns:
        bool: True if the titles are similar above the specified threshold; False otherwise.
    """
    if not correct_title or not raw_title:
        raise ValueError("Both titles must be provided.")
    if not isinstance(correct_title, str) or not isinstance(raw_title, str):
        raise TypeError("Both titles must be strings.")
    return Levenshtein.ratio(correct_title.lower(), raw_title.lower()) >= threshold


def sort(torrents: List[Torrent]) -> List[Torrent]:
    """
    Sorts a list of Torrent objects based on their rank in descending order.

    Args:
        torrents (List[Torrent]): The list of Torrent objects to sort.

    Returns:
        List[Torrent]: The sorted list of Torrent objects by rank.
    """
    return sorted(torrents, key=lambda t: t.rank, reverse=True)
