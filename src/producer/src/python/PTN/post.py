#!/usr/bin/env python

# Post-processing functions that run after the main parsing.

from . import re
from .extras import link_patterns, complete_series
from .patterns import episode_name_pattern, langs, patterns, pre_website_encoder_pattern

# Before excess functions (before we split what was unmatched in the title into a list).
# They all take in the parse object and what was unmatched, and must return the latter minus
# what they used.


# Try and find the episode name.
def try_episode_name(self, unmatched):
    match = re.findall(episode_name_pattern, unmatched)
    # First we see if there's a match in unmatched, then we look if it's after an episode, a day,
    # or a year, in the full release title.
    if match:
        match = re.search(
            "(?:"
            + link_patterns(patterns["episode"])
            + "|"
            + patterns["day"]
            + "|"
            + patterns["year"]
            + r")[._\-\s+]*("
            + re.escape(match[0])
            + ")",
            self.torrent_name,
            re.IGNORECASE,
        )
        if match:
            match_s, match_e = match.start(len(match.groups())), match.end(
                len(match.groups())
            )
            match = match.groups()[-1]
            self._part("episodeName", (match_s, match_e), self._clean_string(match))
            unmatched = unmatched.replace(match, "")
    return unmatched


def try_encoder_before_site(self, unmatched):
    match = re.findall(pre_website_encoder_pattern, unmatched.strip())

    if match:
        found_match = None
        for m in match:
            full_title_match = re.search(
                r"[\s\-]("
                + re.escape(m)
                + ")(?:\."
                + link_patterns(patterns["filetype"])
                + ")?$",
                self.torrent_name,
                re.I,
            )
            if full_title_match:
                found_match = full_title_match
                break
        match = found_match
        if match:
            match_s, match_e = match.start(0), match.end(0)
            encoder_and_site = list(
                filter(None, re.split(r"[\-\s\)]", match.groups()[0]))
            )
            if len(encoder_and_site) == 2:
                encoder_raw = encoder_and_site[0]
                site_raw = encoder_and_site[1]
                self._part(
                    "encoder",
                    (match_s, match_e - len(site_raw)),
                    self._clean_string(encoder_raw),
                )
                self._part(
                    "site",
                    (match_s + len(encoder_raw), match_e),
                    self._clean_string(site_raw),
                    overwrite=True,
                )
                unmatched = unmatched.replace(match.group(0), "")

    return unmatched


def remove_complete_series_string(self, unmatched):
    if "title" in self.parts:
        complete_series_regex = link_patterns(complete_series)
        complete_match = re.search(complete_series_regex, self.parts["title"], flags=re.I)
        if complete_match:
            title = self.parts["title"]
            title = title[: complete_match.start()] + title[complete_match.end() :]
            self._part("title", (complete_match.start(), complete_match.end()), self._clean_string(title), overwrite=True)

    return unmatched


post_processing_before_excess = [
    remove_complete_series_string,
    try_episode_name,
    try_encoder_before_site,
]


# After excess functions take in just the parse object, and shouldn't return anything.


# encoder is assumed to be the last element of `excess`, if not already added.
def try_encoder(self):
    if "excess" not in self.parts or "encoder" in self.parts:
        return
    excess = self.parts["excess"]
    if not isinstance(excess, list):
        excess = [excess]

    if excess:
        encoder = excess.pop()
        self._part("encoder", None, encoder, overwrite=True)

    if not excess:
        self.parts.pop("excess")
    else:
        self._part("excess", None, excess, overwrite=True)


# Split encoder name and site, adding the latter to self.parts
def try_site(self):
    if "encoder" not in self.parts or "website" in self.parts:
        return
    encoder = self.parts["encoder"]
    if self.coherent_types:
        encoder = encoder[0]
    pat = r"(\[(.*)\])"
    match = re.findall(pat, encoder, re.IGNORECASE)
    if match:
        match = match[0]
        raw = match[0]
        if match:
            if not re.match(r"[\[\],.+\-]*\Z", match[1], re.IGNORECASE):
                self._part("site", None, match[1])
            self._part("encoder", None, encoder.replace(raw, ""), overwrite=True)


# If this match starts like the language one did, the only match for language
# and subtitles is a list of langs directly followed by a subs-string. When this
# is true, they would both match on it, but what it likely means is that all the
# langs are language, and the subs string just indicates the existance of subtitles.
# (e.g. Ita.Eng.MSubs would match Ita and Eng for language and subs - this makes
# subs only become MSubs, and leaves language as Ita and Eng)
def fix_same_subtitles_language_match(self):
    if (
        "language" in self.part_slices
        and "subtitles" in self.part_slices
        and self.part_slices["language"][0] == self.part_slices["subtitles"][0]
    ):
        subs = self.parts["subtitles"][-1]
        if self.standardise:
            subs = "Available"
        self._part("subtitles", None, subs, overwrite=True)


# If there are no languages, but subtitles were matched, we should assume the first lang
# is the actual language, and remove it from the subtitles.
def fix_subtitles_no_language(self):
    if (
        "language" not in self.parts
        and "subtitles" in self.parts
        and isinstance(self.parts["subtitles"], list)
        and len(self.parts["subtitles"]) > 1
    ):
        self._part("language", None, self.parts["subtitles"][0])
        self._part("subtitles", None, self.parts["subtitles"][1:], overwrite=True)


# Language matches, to support multi-language releases that have the audio with each
# language, will contain audio info (or simply extra strings like 'dub').
# We remove non-lang matching items from this list.
def filter_non_languages(self):
    if "language" in self.parts and isinstance(self.parts["language"], list):
        languages = list(self.parts["language"])
        for lang in self.parts["language"]:
            matched = False
            for (lang_regex, lang_clean) in langs:
                if re.match(lang_regex, lang, re.IGNORECASE):
                    matched = True
                    break
            if not matched:
                languages.remove(lang)

        self._part("language", self.part_slices["language"], languages, overwrite=True)


def try_vague_season_episode(self):
    title = self.parts["title"]
    m = re.search("(\d{1,2})-(\d{1,2})$", title)
    if m:
        if "season" not in self.parts and "episode" not in self.parts:
            new_title = title[: m.start()]
            offset = self.part_slices["title"][0]
            # Setting the match slices here doesn't actually matter, but good practice.
            self._part(
                "season", (offset + m.start(1), offset + m.end(1)), int(m.group(1))
            )
            self._part(
                "episode", (offset + m.start(2), offset + m.end(2)), int(m.group(2))
            )
            self._part(
                "title",
                (offset, offset + len(new_title)),
                self._clean_string(new_title),
                overwrite=True,
            )


# Probably for movies like 1917, where the title is just the year (would need the release year to also be absent)
def use_year_as_title_if_absent(self):
    if "year" in self.parts and not self.parts.get("title"):
        self._part("title", None, str(self.parts["year"]), overwrite=True)
        self.parts.pop("year")


def remove_empty_parts(self):
    non_empty_parts = {}
    for part in self.parts:
        if self.parts[part] != "":
            non_empty_parts[part] = self.parts[part]

    self.parts = non_empty_parts


post_processing_after_excess = [
    try_encoder,
    try_site,
    fix_same_subtitles_language_match,
    fix_subtitles_no_language,
    filter_non_languages,
    try_vague_season_episode,
    use_year_as_title_if_absent,
    remove_empty_parts,
]
