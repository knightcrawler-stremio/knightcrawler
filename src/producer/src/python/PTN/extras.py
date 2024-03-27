#!/usr/bin/env python

# Helper functions and constants for patterns.py

delimiters = "[\.\s\-\+_\/(),]"

langs = [
    ("rus(?:sian)?", "Russian"),
    ("(?:True)?fre?(?:nch)?", "French"),
    ("(?:nu)?ita(?:liano?)?", "Italian"),
    ("castellano|spa(?:nish)?|esp?", "Spanish"),
    ("swedish", "Swedish"),
    ("dk|dan(?:ish)?", "Danish"),
    ("ger(?:man)?|deu(?:tsch)?", "German"),
    ("nordic", "Nordic"),
    ("exyu", "ExYu"),
    ("chs|chi(?:nese)?", "Chinese"),
    ("hin(?:di)?", "Hindi"),
    ("polish|poland|pl", "Polish"),
    ("mandarin", "Mandarin"),
    ("kor(?:ean)?", "Korean"),
    ("ben(?:gali)?|bangla", "Bengali"),
    ("kan(?:nada)?", "Kannada"),
    ("tam(?:il)?", "Tamil"),
    ("tel(?:ugu)?", "Telugu"),
    ("mar(?:athi)?", "Marathi"),
    ("mal(?:ayalam)?", "Malayalam"),
    ("japanese|ja?p", "Japanese"),
    ("interslavic", "Interslavic"),
    ("ara(?:bic)?", "Arabic"),
    ("urdu", "Urdu"),
    ("punjabi", "Punjabi"),
    ("portuguese", "Portuguese"),
    ("albanian?", "Albanian"),
    ("egypt(?:ian)?", "Egyptian"),
    ("en?(?:g(?:lish)?)?", "English"),  # Must be at end, matches just an 'e'
]

genres = [
    ("Sci-?Fi", "Sci-Fi"),
    ("Drama", "Drama"),
    ("Comedy", "Comedy"),
    ("West(?:\.|ern)?", "Western"),
    ("Action", "Action"),
    ("Adventure", "Adventure"),
    ("Thriller", "Thriller"),
]

# Match strings like "complete series" for tv seasons/series, matching within the final title string.
complete_series = [
    r"(?:the\s)?complete\s(?:series|season|collection)$",
    r"(?:the)\scomplete\s?(?:series|season|collection)?$",
]

# Some titles just can't be parsed without breaking everything else, so here
# are known those known exceptions. They are executed when the parsed_title and
# incorrect_parse match within a .parse() dict, removing the latter, and replacing
# the former with actual_title.
exceptions = [
    {
        "parsed_title": "Marvel's Agents of S H I E L D",
        "incorrect_parse": ("title", "Marvel's Agents of S H I E L D"),
        "actual_title": "Marvel's Agents of S.H.I.E.L.D.",
    },
    {
        "parsed_title": "Marvels Agents of S H I E L D",
        "incorrect_parse": ("title", "Marvels Agents of S H I E L D"),
        "actual_title": "Marvel's Agents of S.H.I.E.L.D.",
    },
    {
        "parsed_title": "Magnum P I",
        "incorrect_parse": ("title", "Magnum P I"),
        "actual_title": "Magnum P.I.",
    },
]

# Patterns that should only try to be matched after the 'title delimiter', either a year
# or a season. So if we have a language in the title it won't cause issues by getting matched.
# Empty list indicates to always do so, as opposed to matching specific regexes.
patterns_ignore_title = {
    "language": [],
    "audio": ["LiNE"],
    "network": ["Hallmark"],
    "untouched": [],
    "internal": [],
    "limited": [],
    "proper": [],
    "extended": [],
}


channels = [(1, 0), (2, 0), (5, 0), (5, 1), (6, 1), (7, 1)]


# Return tuple with regexes for audio name with appended channel types, and without any channels
def get_channel_audio_options(patterns_with_names):
    options = []
    for (audio_pattern, name) in patterns_with_names:
        for (speakers, subwoofers) in channels:
            options.append(
                (
                    "((?:{}){}*{}[. \-]?{}(?:ch)?)".format(
                        audio_pattern, delimiters, speakers, subwoofers
                    ),
                    "{} {}.{}".format(name, speakers, subwoofers),
                )
            )
        options.append(
            ("({})".format(audio_pattern), name)
        )  # After for loop, would match first

    return options


def prefix_pattern_with(prefixes, pattern_options, between="", optional=False):
    if optional:
        optional_char = "?"
    else:
        optional_char = ""
    options = []
    if not isinstance(prefixes, list):
        prefixes = [prefixes]
    for prefix in prefixes:
        if not isinstance(pattern_options, list):
            pattern_options = [pattern_options]
        for pattern_option in pattern_options:
            if isinstance(pattern_option, str):
                options.append(
                    "(?:{}){}(?:{})?({})".format(
                        prefix, optional_char, between, pattern_option
                    )
                )
            else:
                options.append(
                    (
                        "(?:{}){}(?:{})?({})".format(
                            prefix, optional_char, between, pattern_option[0]
                        ),
                    )
                    + pattern_option[1:]
                )

    return options


def suffix_pattern_with(suffixes, pattern_options, between="", optional=False):
    if optional:
        optional_char = "?"
    else:
        optional_char = ""
    options = []
    if not isinstance(suffixes, list):
        suffixes = [suffixes]
    for suffix in suffixes:
        if not isinstance(pattern_options, list):
            pattern_options = [pattern_options]
        for pattern_option in pattern_options:
            if isinstance(pattern_option, tuple):
                options.append(
                    (
                        "({})(?:{})?(?:{}){}".format(
                            pattern_option[0], between, suffix, optional_char
                        ),
                    )
                    + pattern_option[1:]
                )
            else:
                options.append(
                    "({})(?:{})?(?:{}){}".format(
                        pattern_option, between, suffix, optional_char
                    )
                )

    return options


# Link a regex-tuple list into a single regex (to be able to use elsewhere while
# maintaining standardisation functionality).
def link_patterns(pattern_options):
    if not isinstance(pattern_options, list):
        return pattern_options
    return (
        "(?:"
        + "|".join(
            [
                pattern_option[0]
                if isinstance(pattern_option, tuple)
                else pattern_option
                for pattern_option in pattern_options
            ]
        )
        + ")"
    )
