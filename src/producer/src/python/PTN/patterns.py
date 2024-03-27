#!/usr/bin/env python

# Patterns are either just a regex, or a tuple (or list of tuples) that contain the regex
# to match, (optional) what it should be replaced with (None if to not replace), and
# (optional) a string function's name to transform the value after everything (None if
# to do nothing). The transform can also be a tuple (or list of tuples) with function names
# and list of arguments.
# The list of regexes all get matched, but only the first gets added to the returning info,
# the rest are just matched to be removed from `excess`.

from .extras import (
    delimiters,
    genres,
    get_channel_audio_options,
    langs,
    link_patterns,
    suffix_pattern_with,
)

season_range_pattern = (
    "(?:Complete"
    + delimiters
    + "*)?"
    + delimiters
    + "*(?:s(?:easons?)?)"
    + delimiters
    + "*(?:s?[0-9]{1,2}[\s]*(?:(?:\-|(?:\s*to\s*))[\s]*s?[0-9]{1,2}))(?:"
    + delimiters
    + "*Complete)?"
)

year_pattern = "(?:19[0-9]|20[0-2])[0-9]"
month_pattern = "0[1-9]|1[0-2]"
day_pattern = "[0-2][0-9]|3[01]"

episode_name_pattern = (
    "((?:[Pp](?:ar)?t"
    + delimiters
    + "*[0-9]|(?:[A-Za-z]|[0-9])[a-z]*(?:"
    + delimiters
    + "|$))+)"
)
pre_website_encoder_pattern = r"[^\s\.\[\]\-\(\)]+\)\s*\[[^\s\-]+\]|[^\s\.\[\]\-\(\)]+\s*(?:-\s)?[^\s\.\[\]\-]+$"

# Forces an order to go by the regexes, as we want this to be deterministic (different
# orders can generate different matchings). e.g. "doctor_who_2005..." in input.json
patterns_ordered = [
    "resolution",
    "quality",
    "season",
    "episode",
    "year",
    "month",
    "day",
    "codec",
    "audio",
    "region",
    "extended",
    "hardcoded",
    "proper",
    "repack",
    "filetype",
    "widescreen",
    "sbs",
    "site",
    "documentary",
    "language",
    "subtitles",
    "unrated",
    "size",
    "bitDepth",
    "3d",
    "internal",
    "readnfo",
    "network",
    "fps",
    "hdr",
    "limited",
    "remastered",
    "directorsCut",
    "upscaled",
    "untouched",
    "remux",
    "internationalCut",
    "genre",
]


# Some patterns overlap with others. Season & episode do this a lot. Without something like this, we'd get issues like
# the Avatar test: ... Complete Series 1080p ... 'Series 10' would be matched as a season, but the 10 is
# from 1080p, which also gets matched.
patterns_allow_overlap = [
    "season",
    "episode",
    "language",
    "subtitles",
    "sbs"
]

patterns = {}
patterns["episode"] = [
    r"(?<![a-z])(?:e|ep)(?:\(?[0-9]{1,2}(?:-?(?:e|ep)?(?:[0-9]{1,2}))?\)?)(?![0-9])",
    # Very specific as it could match too liberally
    r"\s\-\s\d{1,3}\s",
    r"\b[0-9]{1,2}x([0-9]{2})\b",
    r"\bepisod(?:e|io)" + delimiters + r"\d{1,2}\b",
]
# If adding season patterns, remember to look at episode, as it uses the last few!
patterns["season"] = [
    r"\b(?:Seasons?)"
    + delimiters
    + r"(\d{1,2})" + "(?:(?:" + delimiters + r"|&|and|to){1,3}(\d{1,2})){2,}\b",
    r"\ss?(\d{1,2})\s\-\s\d{1,2}\s",  # Avoids matching some anime releases season and episode as a season range
    r"\b" + season_range_pattern + r"\b",  # Describes season ranges
    r"(?:s\d{1,2}[.+\s]*){2,}\b",  # for S01.S02.etc. patterns
    # Describes season, optionally with complete or episode
    r"\b(?:Complete"
    + delimiters
    + ")?s([0-9]{1,2})"
    + link_patterns(patterns["episode"])
    + r"?\b",
    r"\b([0-9]{1,2})x[0-9]{2}\b",  # Describes 5x02, 12x15 type descriptions
    "[0-9]{1,2}(?:st|nd|rd|th)" + delimiters + "season",
    "Series" + delimiters + "\d{1,2}",
    r"\b(?:Complete"
    + delimiters
    + r")?Season[\. -][0-9]{1,2}\b",  # Describes Season.15 type descriptions
]
# The first 4 season regexes won't have 'Part' in them.
patterns["episode"] += [
    link_patterns(patterns["season"][6:])
    + delimiters
    + "*P(?:ar)?t"
    + delimiters
    + "*(\d{1,3})"
]
patterns["year"] = year_pattern
patterns["month"] = "(?:{year}){d}({month}){d}(?:{day})".format(
    d=delimiters, year=year_pattern, month=month_pattern, day=day_pattern
)
patterns["day"] = "(?:{year}){d}(?:{month}){d}({day})".format(
    d=delimiters, year=year_pattern, month=month_pattern, day=day_pattern
)
# resolution pattern according to https://ihax.io/display-resolution-explained/ and GPT4.
# order from highest to lowest due to some torrent name having '4K HD' in them but its technically 4K.
patterns["resolution"] = [
    (r"([0-9]{3,4}(?:p|i))", None, "lower"),  # Generic pattern for resolutions like 480p, 720p, 1080p, etc.
    (r"(8K|7680{d}?x{d}?4320p?)".format(d=delimiters), "8K"),  # Pattern for 8K
    (r"(5K|5120{d}?x{d}?2880p?)".format(d=delimiters), "5K"),  # Pattern for 5K
    (r"(4K UHD|UHD|3840{d}?x{d}?2160p?)".format(d=delimiters), "2160p"),  # Pattern for 4K UHD / 2160p
    (r"(4K|4096{d}?x{d}?2160p?)".format(d=delimiters), "4K"),  # Pattern for 4K / Cinema 4K
    (r"(QHD|QuadHD|WQHD|2560{d}?x{d}?1440p?)".format(d=delimiters), "1440p"),  # Pattern for QHD / 1440p
    (r"(2K|2048{d}?x{d}?1080p?)".format(d=delimiters), "2K"),  # Pattern for 2K
    (r"(Full HD|FHD|1920{d}?x{d}?1080p?)".format(d=delimiters), "1080p"),  # Pattern for Full HD / 1080p
    (r"(HD|1280{d}?x{d}?720p?)".format(d=delimiters), "720p"),  # Pattern for HD / 720p
    (r"(qHD)", "540p"),  # Pattern for quarter High Definition
    (r"(SD)", "480p"),  # Pattern for Standard Definition
]

patterns["quality"] = [
    ("WEB[ -\.]?DL(?:Rip|Mux)?|HDRip", "WEB-DL"),
    # Match WEB-DL's first as they can show up with others.
    ("WEB[ -]?Cap", "WEBCap"),
    ("W[EB]B[ -]?(?:Rip)|WEB", "WEBRip"),
    ("(?:HD)?CAM(?:-?Rip)?", "Cam"),
    ("(?:HD)?TS|TELESYNC|PDVD|PreDVDRip", "Telesync"),
    ("WP|WORKPRINT", "Workprint"),
    ("(?:HD)?TC|TELECINE", "Telecine"),
    ("(?:DVD)?SCR(?:EENER)?|BDSCR", "Screener"),
    ("DDC", "Digital Distribution Copy"),
    ("DVD-?(?:Rip|Mux)", "DVD-Rip"),
    ("DVDR|DVD-Full|Full-rip", "DVD-R"),
    ("PDTV|DVBRip", "PDTV"),
    ("DSR(?:ip)?|SATRip|DTHRip", "DSRip"),
    ("AHDTV(?:Mux)?", "AHDTV"),
    ("HDTV(?:Rip)?", "HDTV"),
    ("D?TVRip|DVBRip", "TVRip"),
    ("VODR(?:ip)?", "VODRip"),
    ("HD-Rip", "HD-Rip"),
    ("Blu-?Ray{d}Rip|BDR(?:ip)?".format(d=delimiters), "BDRip"),
    ("Blu-?Ray|(?:US|JP)?BD(?:remux)?", "Blu-ray"),
    ("BR-?Rip", "BRRip"),
    ("HDDVD", "HD DVD"),
    # Match this last as it can show up with others.
    ("PPV(?:Rip)?", "Pay-Per-View Rip"),
]
patterns["network"] = [
    ("ATVP", "Apple TV+"),
    ("AMZN|Amazon", "Amazon Studios"),
    ("NF|Netflix", "Netflix"),
    ("NICK", "Nickelodeon"),
    ("RED", "YouTube Premium"),
    ("DSNY?P", "Disney Plus"),
    ("DSNY", "DisneyNOW"),
    ("HMAX", "HBO Max"),
    ("HBO", "HBO"),
    ("HULU", "Hulu Networks"),
    ("MS?NBC", "MSNBC"),
    ("DCU", "DC Universe"),
    ("ID", "Investigation Discovery"),
    ("iT", "iTunes"),
    ("AS", "Adult Swim"),
    ("CRAV", "Crave"),
    ("CC", "Comedy Central"),
    ("SESO", "Seeso"),
    ("VRV", "VRV"),
    ("PCOK", "Peacock"),
    ("CBS", "CBS"),
    ("iP", "BBC iPlayer"),
    ("NBC", "NBC"),
    ("AMC", "AMC"),
    ("PBS", "PBS"),
    ("STAN", "Stan."),
    ("RTE", "RTE Player"),
    ("CR", "Crunchyroll"),
    ("ANPL", "Animal Planet Live"),
    ("DTV", "DirecTV Stream"),
    ("VICE", "VICE"),
]
patterns["network"] = suffix_pattern_with(
    link_patterns(patterns["quality"]), patterns["network"], delimiters
)
# Not all networks always show up just before the quality, so if they're unlikely to clash,
# they should be added here.
patterns["network"] += [
    ("BBC", "BBC"),
    ("Hoichoi", "Hoichoi"),
    ("Zee5", "ZEE5"),
    ("Hallmark", "Hallmark"),
    ("Sony\s?LIV", "SONY LIV"),
]
patterns["codec"] = [
    ("xvid", "Xvid"),
    ("av1", "AV1"),
    ("[hx]{d}?264".format(d=delimiters), "H.264"),
    ("AVC", "H.264"),
    ("HEVC(?:{d}Main{d}?10P?)".format(d=delimiters), "H.265 Main 10"),
    (
        "[hx]{d}?265".format(d=delimiters),
        "H.265",
    ),  # Separate from HEVC so if both are present, it won't pollute excess.
    ("HEVC", "H.265"),
    ("[h]{d}?263".format(d=delimiters), "H.263"),
    ("VC-1", "VC-1"),
    ("MPEG{d}?2".format(d=delimiters), "MPEG-2"),
]
patterns["audio"] = get_channel_audio_options(
    [
        ("TrueHD", "Dolby TrueHD"),
        ("Atmos", "Dolby Atmos"),
        ("DD{d}?EX".format(d=delimiters), "Dolby Digital EX"),
        ("DD|AC{d}?3|DolbyD".format(d=delimiters), "Dolby Digital"),
        ("DDP|E{d}?AC{d}?3|EC{d}?3".format(d=delimiters), "Dolby Digital Plus"),
        (
            "DTS{d}?HD(?:{d}?(?:MA|Masters?(?:{d}Audio)?))".format(d=delimiters),
            "DTS-HD MA",
        ),
        ("DTSMA", "DTS-HD MA"),
        ("DTS{d}?HD".format(d=delimiters), "DTS-HD"),
        ("DTS{d}?ES".format(d=delimiters), "DTS-ES"),
        ("DTS{d}?EX".format(d=delimiters), "DTS-EX"),
        ("DTS{d}?X".format(d=delimiters), "DTS:X"),
        ("DTS", "DTS"),
        ("HE{d}?AAC".format(d=delimiters), "HE-AAC"),
        ("HE{d}?AACv2".format(d=delimiters), "HE-AAC v2"),
        ("AAC{d}?LC".format(d=delimiters), "AAC-LC"),
        ("AAC", "AAC"),
        ("Dual{d}Audios?".format(d=delimiters), "Dual"),
        ("Custom{d}Audios?".format(d=delimiters), "Custom"),
        ("FLAC", "FLAC"),
        ("OGG", "OGG"),
    ]
) + [
    ("7.1(?:{d}?ch(?:annel)?(?:{d}?Audio)?)?".format(d=delimiters), "7.1"),
    ("5.1(?:{d}?ch(?:annel)?(?:{d}?Audio)?)?".format(d=delimiters), "5.1"),
    ("MP3", None, "upper"),
    ("2.0(?:{d}?ch(?:annel)?(?:{d}?Audio)?)?|2CH|stereo".format(d=delimiters), "Dual"),
    ("1{d}?Ch(?:annel)?(?:{d}?Audio)?".format(d=delimiters), "Mono"),
    ("(?:Original|Org)" + delimiters + "Aud(?:io)?", "Original"),
    ("LiNE", "LiNE"),
]
patterns["region"] = ("R[0-9]", None, "upper")
patterns["extended"] = "(EXTENDED(:?.CUT)?)"
patterns["hardcoded"] = "HC"
patterns["proper"] = "PROPER"
patterns["repack"] = "REPACK"
patterns["fps"] = "([1-9][0-9]{1,2})" + delimiters + "*fps"
patterns["filetype"] = [
    (r"\.?(MKV|AVI|(?:SRT|SUB|SSA)$)", None, "upper"),
    ("MP-?4", "MP4"),
    (r"\.?(iso)$", "ISO"),
]
patterns["widescreen"] = "WS"
patterns["site"] = [r"^(\[ ?([^\]]+?)\s?\])", r"^((?:www\.)?[\w-]+\.[\w]{2,4})\s+-\s*"]

lang_list_pattern = (
    r"\b(?:"
    + link_patterns(langs)
    + "(?:"
    + delimiters
    + "+(?:dub(?:bed)?|"
    + link_patterns(patterns["audio"])
    + "))?"
    + "(?:"
    + delimiters
    + r"+|\b))"
)
subs_list_pattern = r"(?:" + link_patterns(langs) + delimiters + "*)"

patterns["subtitles"] = [
    # Below must stay first, see patterns["language"]
    "sub(?:title|bed)?s?{d}*{langs}+".format(d=delimiters, langs=subs_list_pattern),
    "(?:soft{d}*)?{langs}+(?:(?:m(?:ulti(?:ple)?)?{d}*)?sub(?:title|bed)?s?)".format(
        d=delimiters, langs=subs_list_pattern
    ),
    ("VOSTFR", ["French"]),
    # The following are patterns just for the 'subs' strings. Add normal sub stuff above.
    # Need a pattern just for subs, and can't just make above regexes * over + as we want
    # just 'subs' to match last.
    # The second-last one must stay second-last, see patterns["language"]
    "(?:m(?:ulti(?:ple)?)?{d}*)sub(?:title|bed)?s?".format(d=delimiters),
    "(?:m(?:ulti(?:ple)?)?[\.\s\-\+_\/]*)?sub(?:title|bed)?s?{d}*".format(d=delimiters),
]
# Language takes precedence over subs when ambiguous - if we have a lang match, and
# then a subtitles match starting with subs, the first langs are languages, and the
# rest will be left as subtitles. Otherwise, don't match if there are subtitles matches
# after the langs.
patterns["language"] = [
    "("
    + lang_list_pattern
    + "+)(?:"
    + delimiters
    + "*"
    + patterns["subtitles"][0]
    + ")",
    "("
    + lang_list_pattern
    + "+)"
    + ""
    + "(?!"
    + delimiters
    + "*"
    + link_patterns(patterns["subtitles"])
    + ")",
    "("
    + lang_list_pattern
    + "+)(?:"
    + delimiters
    + "*"
    + patterns["subtitles"][-2]
    + ")",
]
patterns["sbs"] = [("Half-SBS", "Half SBS"), ("SBS", None, "upper")]
patterns["unrated"] = "UNRATED"
patterns["size"] = (
    "\d+(?:\.\d+)?\s?(?:GB|MB)",
    None,
    [("upper", []), ("replace", [" ", ""])],
)
patterns["bitDepth"] = "(8|10)-?bits?"
patterns["3d"] = "3D"
patterns["internal"] = "iNTERNAL"
patterns["readnfo"] = "READNFO"
patterns["hdr"] = "HDR(?:10)?"
patterns["documentary"] = "DOCU(?:menta?ry)?"
patterns["limited"] = "LIMITED"
patterns["remastered"] = "REMASTERED"
patterns["directorsCut"] = "DC|Director'?s.?Cut"
patterns["upscaled"] = "(?:AI{d}*)?upscaled?".format(d=delimiters)
patterns["untouched"] = "untouched"
patterns["remux"] = "REMUX"
patterns["internationalCut"] = "International{d}Cut".format(d=delimiters)
# Spaces are only allowed before the genre list if after a word boundary or punctuation
patterns["genre"] = (
    r"\b\s*[\(\-\]]+\s*((?:" + link_patterns(genres) + delimiters + r"?)+)\b"
)

types = {
    "season": "integer",
    "episode": "integer",
    "bitDepth": "integer",
    "year": "integer",
    "month": "integer",
    "day": "integer",
    "fps": "integer",
    "extended": "boolean",
    "hardcoded": "boolean",
    "proper": "boolean",
    "repack": "boolean",
    "widescreen": "boolean",
    "unrated": "boolean",
    "3d": "boolean",
    "internal": "boolean",
    "readnfo": "boolean",
    "documentary": "boolean",
    "hdr": "boolean",
    "limited": "boolean",
    "remastered": "boolean",
    "directorsCut": "boolean",
    "upscaled": "boolean",
    "untouched": "boolean",
    "remux": "boolean",
    "internationalCut": "boolean",
}
