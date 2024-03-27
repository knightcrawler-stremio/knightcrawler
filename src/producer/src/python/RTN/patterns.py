from typing import Any, Dict, List

import regex


def compile_patterns(patterns):
    return [regex.compile(pattern, regex.IGNORECASE) for pattern in patterns]


# Pattern for identifying unwanted quality. This will set `parsed_data.fetch`.
IS_TRASH_COMPILED = compile_patterns(
    [
        r"\b(?:H[DQ][ .-]*)?CAM(?:H[DQ])?(?:[ .-]*Rip)?\b",
        r"\b(?:H[DQ][ .-]*)?S[ .-]*print\b",
        r"\b(?:HD[ .-]*)?T(?:ELE)?S(?:YNC)?(?:Rip)?\b",
        r"\b(?:HD[ .-]*)?T(?:ELE)?C(?:INE)?(?:Rip)?\b",
        r"\bP(?:re)?DVD(?:Rip)?\b",
        r"\b(?:DVD?|BD|BR)?[ .-]*Scr(?:eener)?\b",
        r"\bVHS\b",
        r"\bHD[ .-]*TV(?:Rip)\b",
        r"\bDVB[ .-]*(?:Rip)?\b",
        r"\bSAT[ .-]*Rips?\b",
        r"\bTVRips?\b",
        r"\bR5|R6\b",
        r"\b(DivX|XviD)\b",
        r"\b(?:Deleted[ .-]*)?Scene(?:s)?\b",
        r"\bTrailers?\b",
        r"\b((Half.)?SBS|3D)\b",
        r"\bWEB[ .-]?DL[ .-]?Rip\b",
    ]
)

# Pattern for checking multi-audio in a torrent's title.
MULTI_AUDIO_COMPILED = compile_patterns(
    [
        r"\bmulti(?:ple)?[ .-]*(?:lang(?:uages?)?|audio|VF2)?\b",
        r"\btri(?:ple)?[ .-]*(?:audio|dub\w*)\b",
        r"\bdual[ .-]*(?:au?$|[aá]udio|line)\b",
        r"\b(?:audio|dub(?:bed)?)[ .-]*dual\b",
        r"\b(?:DUBBED|dublado|dubbing|DUBS?)\b",
    ]
)

# Pattern for checking multi-subtitle in a torrent's title.
MULTI_SUBTITLE_COMPILED = compile_patterns(
    [
        r"\bmulti(?:ple)?[ .-]*(?:lang(?:uages?)?)?\b",
        r"\bdual\b(?![ .-]*sub)",
        r"\bengl?(?:sub[A-Z]*)?\b",
        r"\beng?sub[A-Z]*\b",
    ]
)

# Pattern for checking HDR/Dolby video in a torrent's title.
HDR_DOLBY_VIDEO_COMPILED = [
    (regex.compile(pattern, regex.IGNORECASE), value)
    for pattern, value in [
        (r"\bDV\b|dolby.?vision|\bDoVi\b", "DV"),
        (r"HDR10(?:\+|plus)", "HDR10+"),
        (r"\bHDR(?:10)?\b", "HDR"),
    ]
]

# Pattern for identifying a complete series.
COMPLETE_SERIES_COMPILED = compile_patterns(
    [
        r"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bbox[ .-]?set\b",
        r"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bmini[ .-]?series\b",
        r"(?:\bthe\W)?(?:\bcomplete|full|all)\b.*\b(?:series|seasons|collection|episodes|set|pack|movies)\b",
        r"\b(?:series|seasons|movies?)\b.*\b(?:complete|collection)\b",
        r"(?:\bthe\W)?\bultimate\b[ .]\bcollection\b",
        r"\bcollection\b.*\b(?:set|pack|movies)\b",
        r"\bcollection\b",
        r"duology|trilogy|quadr[oi]logy|tetralogy|pentalogy|hexalogy|heptalogy|anthology|saga",
    ]
)

# Patterns for parsing episodes.
EPISODE_PATTERNS = [
    (
        regex.compile(
            r"(?:[\W\d]|^)e[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|e){1,2}[ .]?\d{1,3})+)(?:\W|$)", regex.IGNORECASE
        ),
        "range",
    ),
    (
        regex.compile(
            r"(?:[\W\d]|^)ep[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|ep){1,2}[ .]?\d{1,3})+)(?:\W|$)", regex.IGNORECASE
        ),
        "range",
    ),
    (
        regex.compile(r"(?:[\W\d]|^)\d+[xх][ .]?[([]?(\d{1,3}(?:[ .]?[xх][ .]?\d{1,3})+)(?:\W|$)", regex.IGNORECASE),
        "range",
    ),
    (
        regex.compile(
            r"(?:[\W\d]|^)(?:episodes?|[Сс]ерии:?)[ .]?[([]?(\d{1,3}(?:[ .+]*[&+][ .]?\d{1,3})+)(?:\W|$)",
            regex.IGNORECASE,
        ),
        "range",
    ),
    (regex.compile(r"[([]?(?:\D|^)(\d{1,3}[ .]?ao[ .]?\d{1,3})[)\]]?(?:\W|$)", regex.IGNORECASE), "range"),
    (
        regex.compile(
            r"(?:[\W\d]|^)(?:e|eps?|episodes?|[Сс]ерии:?|\d+[xх])[ .]*[([]?(\d{1,3}(?:-\d{1,3})+)(?:\W|$)",
            regex.IGNORECASE,
        ),
        "range",
    ),
    (
        regex.compile(
            r"(?:\W|^)[st]\d{1,2}[. ]?[xх-]?[. ]?(?:e|x|х|ep|-|\.)[. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)", regex.IGNORECASE
        ),
        "array(integer)",
    ),
    (regex.compile(r"\b[st]\d{2}(\d{2})\b", regex.IGNORECASE), "array(integer)"),
    (regex.compile(r"(?:\W|^)(\d{1,3}(?:[ .]*~[ .]*\d{1,3})+)(?:\W|$)", regex.IGNORECASE), "range"),
    (regex.compile(r"-\s(\d{1,3}[ .]*-[ .]*\d{1,3})(?!-\d)(?:\W|$)", regex.IGNORECASE), "range"),
    (regex.compile(r"s\d{1,2}\s?\((\d{1,3}[ .]*-[ .]*\d{1,3})\)", regex.IGNORECASE), "range"),
    (regex.compile(r"(?:^|\/)\d{1,2}-(\d{2})\b(?!-\d)"), "array(integer)"),
    (regex.compile(r"(?<!\d-)\b\d{1,2}-(\d{2})(?=\.\w{2,4}$)"), "array(integer)"),
    (
        regex.compile(
            r"(?<!seasons?|[Сс]езони?)\W(?:[ .([-]|^)(\d{1,3}(?:[ .]?[,&+~][ .]?\d{1,3})+)(?:[ .)\]-]|$)",
            regex.IGNORECASE,
        ),
        "range",
    ),
    (
        regex.compile(
            r"(?<!seasons?|[Сс]езони?)\W(?:[ .([-]|^)(\d{1,3}(?:-\d{1,3})+)(?:[ .)(\]]|-\D|$)", regex.IGNORECASE
        ),
        "range",
    ),
    (regex.compile(r"\bEp(?:isode)?\W+\d{1,2}\.(\d{1,3})\b", regex.IGNORECASE), "array(integer)"),
    (
        regex.compile(
            r"(?:\b[ée]p?(?:isode)?|[Ээ]пизод|[Сс]ер(?:ии|ия|\.)?|cap(?:itulo)?|epis[oó]dio)[. ]?[-:#№]?[. ]?(\d{1,4})(?:[abc]|v0?[1-4]|\W|$)",
            regex.IGNORECASE,
        ),
        "array(integer)",
    ),
    (
        regex.compile(r"\b(\d{1,3})(?:-?я)?[ ._-]*(?:ser(?:i?[iyj]a|\b)|[Сс]ер(?:ии|ия|\.)?)", regex.IGNORECASE),
        "array(integer)",
    ),
    (regex.compile(r"(?:\D|^)\d{1,2}[. ]?[xх][. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)"), "array(integer)"),
    (regex.compile(r"[[(]\d{1,2}\.(\d{1,3})[)\]]"), "array(integer)"),
    (regex.compile(r"\b[Ss]\d{1,2}[ .](\d{1,2})\b"), "array(integer)"),
    (regex.compile(r"-\s?\d{1,2}\.(\d{2,3})\s?-"), "array(integer)"),
    (regex.compile(r"(?<=\D|^)(\d{1,3})[. ]?(?:of|из|iz)[. ]?\d{1,3}(?=\D|$)", regex.IGNORECASE), "array(integer)"),
    (regex.compile(r"\b\d{2}[ ._-](\d{2})(?:.F)?\.\w{2,4}$"), "array(integer)"),
    (regex.compile(r"(?<!^)\[(\d{2,3})\](?!(?:\.\w{2,4})?$)"), "array(integer)"),
]


def check_pattern(patterns: list[regex.Pattern], raw_title: str) -> bool:
    return any(pattern.search(raw_title) for pattern in patterns)


def check_hdr_dolby_video(raw_title: str) -> str:
    """Returns the HDR/Dolby video type if found in the title."""
    for pattern, value in HDR_DOLBY_VIDEO_COMPILED:
        if pattern.search(raw_title):
            return value
    return ""


def range_transform(raw_title: str) -> set[int]:
    """
    Expands a range string into a list of individual episode numbers.
    Example input: '1-3', '1&2&3', '1E2E3'
    Returns: [1, 2, 3]
    """
    episodes = set()
    # Split input string on non-digit characters, filter empty strings.
    parts = [part for part in regex.split(r"\D+", raw_title) if part]
    # Convert parts to integers, ignoring non-numeric parts.
    episode_nums = [int(part) for part in parts if part.isdigit()]
    # If it's a simple range (e.g., '1-3'), expand it.
    if len(episode_nums) == 2 and episode_nums[0] < episode_nums[1]:
        episodes.update(range(episode_nums[0], episode_nums[1] + 1))
    else:
        episodes.update(episode_nums)
    return episodes


def extract_episodes(raw_title: str) -> List[int]:
    """Extract episode numbers from the title."""
    episodes = set()
    for compiled_pattern, transform in EPISODE_PATTERNS:
        matches = compiled_pattern.findall(raw_title)
        for match in matches:
            if transform == "range":
                if isinstance(match, tuple):
                    for m in match:
                        episodes.update(range_transform(m))
                else:
                    episodes.update(range_transform(match))
            elif transform == "array(integer)":
                normalized_match = [match] if isinstance(match, str) else match
                episodes.update(int(m) for m in normalized_match if m.isdigit())
    return sorted(episodes)


def parse_extras(raw_title: str) -> Dict[str, Any]:
    """
    Parses the input string to extract additional information relevant to RTN processing.

    Parameters:
    - raw_title (str): The original title of the torrent to analyze.

    Returns:
    - Dict[str, Any]: A dictionary containing extracted information from the torrent title.
    """
    if not raw_title or not isinstance(raw_title, str):
        raise TypeError("The input title must be a non-empty string.")

    return {
        "is_multi_audio": check_pattern(MULTI_AUDIO_COMPILED, raw_title),
        "is_multi_subtitle": check_pattern(MULTI_SUBTITLE_COMPILED, raw_title),
        "is_complete": check_pattern(COMPLETE_SERIES_COMPILED, raw_title),
        "hdr": check_hdr_dolby_video(raw_title) or False,
        "episode": extract_episodes(raw_title),
    }
