import regex

from .models import ParsedData, SettingsModel
from .patterns import IS_TRASH_COMPILED


def check_trash(raw_title: str) -> bool:
    """Check if the title contains unwanted patterns."""
    if not raw_title or not isinstance(raw_title, str):
        raise TypeError("The input title must be a non-empty string.")
    # True if we find any of the trash patterns in the title.
    # You can safely remove any title from being scraped if this returns True!
    return any(pattern.search(raw_title) for pattern in IS_TRASH_COMPILED)


def check_fetch(data: ParsedData, settings: SettingsModel) -> bool:
    """Check user settings and unwanted quality to determine if torrent should be fetched."""
    if check_trash(data.raw_title):
        return False
    if settings.require and any(
        pattern.search(data.raw_title) for pattern in settings.require if pattern  # type: ignore
    ):
        return True
    if settings.exclude and any(
        pattern.search(data.raw_title) for pattern in settings.exclude if pattern  # type: ignore
    ):
        return False
    return all(
        [
            fetch_resolution(data, settings),
            fetch_quality(data, settings),
            fetch_audio(data, settings),
            fetch_codec(data, settings),
            fetch_other(data, settings),
        ]
    )


def fetch_quality(data: ParsedData, settings: SettingsModel) -> bool:
    """Check if the quality is fetchable based on user settings."""
    if not settings.custom_ranks["webdl"].fetch and "WEB-DL" in data.quality:
        return False
    if not settings.custom_ranks["remux"].fetch and data.remux:
        return False
    if not settings.custom_ranks["ddplus"].fetch and "Dolby Digital Plus" in data.audio:
        return False
    if not settings.custom_ranks["aac"].fetch and "AAC" in data.audio:
        return False
    return True


def fetch_resolution(data: ParsedData, settings: SettingsModel) -> bool:
    """Check if the resolution is fetchable based on user settings."""
    if data.is_4k and not settings.custom_ranks["uhd"].fetch:
        return False
    if "1080p" in data.resolution and not settings.custom_ranks["fhd"].fetch:
        return False
    if "720p" in data.resolution and not settings.custom_ranks["hd"].fetch:
        return False
    if any(res in data.resolution for res in ["576p", "480p"]) and not settings.custom_ranks["sd"].fetch:
        return False
    return True


def fetch_codec(data: ParsedData, settings: SettingsModel) -> bool:
    """Check if the codec is fetchable based on user settings."""
    # May add more to this later...
    if not settings.custom_ranks["av1"].fetch and "AV1" in data.codec:
        return False
    return True


def fetch_audio(data: ParsedData, settings: SettingsModel) -> bool:
    """Check if the audio is fetchable based on user settings."""
    if not data.audio:
        return True

    # Remove unwanted audio concatenations.
    audio: str = data.audio[0]
    audio = regex.sub(r"7.1|5.1|Dual|Mono|Original|LiNE", "", audio).strip()

    if not settings.custom_ranks["truehd"].fetch and audio == "Dolby TrueHD":
        return False
    if not settings.custom_ranks["atmos"].fetch and audio == "Dolby Atmos":
        return False
    if not settings.custom_ranks["ac3"].fetch and audio == "Dolby Digital":
        return False
    if not settings.custom_ranks["dts_x"].fetch and audio == "Dolby Digital EX":
        return False
    if not settings.custom_ranks["ddplus"].fetch and audio == "Dolby Digital Plus":
        return False
    if not settings.custom_ranks["dts_hd_ma"].fetch and audio == "DTS-HD MA":
        return False
    if not settings.custom_ranks["dts_hd"].fetch and audio == "DTS":
        return False
    if not settings.custom_ranks["aac"].fetch and audio == "AAC":
        return False
    return True


def fetch_other(data: ParsedData, settings: SettingsModel) -> bool:
    """Check if the other data is fetchable based on user settings."""
    if not settings.custom_ranks["proper"].fetch and data.proper:
        return False
    if not settings.custom_ranks["repack"].fetch and data.repack:
        return False
    return True
