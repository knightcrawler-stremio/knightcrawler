using System.Web;

namespace Producer.Features.ParseTorrentTitle;

public class ParsedTorrent
{
    [Pattern(Regex = "AMZN")]
    public bool Amazon { get; set; }

    [Pattern(Regex = @"MP3|DDP?\+?|Dual[\- ]Audio|LiNE|D[Tt][Ss](?:-?6[Cc][Hh])?(?:-?HD)?(?:[ \.]?MA)?|AAC(?:\.?2\.0)?|[Aa][Cc]-?3(?:\s?DD)?", Replacements = "., ")]
    public string? Audio { get; set; }

    [Pattern(Regex = @"[257][\s\.][01]", Replacements = " ,.")]
    public string? AudioChannels { get; set; }

    [Pattern(Regex = @"((\d+)\s?bit)", Options = RegexOptions.IgnoreCase)]
    public int? BitDepth { get; set; }

    [Pattern(Regex = "BLURRED")]
    public bool Blurred { get; set; }

    [Pattern(Regex = @"xvid|[xh]\.?26[45]|hevc", Options = RegexOptions.IgnoreCase)]
    public string? Codec { get; set; }

    [Pattern(Regex = "COMPLETE")]
    public bool Complete { get; set; }

    [Pattern(Regex = @"[\s\.-](MKV|AVI|MP4|mkv|avi|mp4)")]
    public string? Container { get; set; }

    [Pattern(Regex = @"ATMOS|Atmos\b")]
    public bool DolbyAtmos { get; set; }

    [Pattern(Regex = "DUBBED")]
    public bool Dubbed { get; set; }

    [Pattern(Regex = @"([Eex]([0-9]{2})(?:[^0-9]|$))")]
    public int? Episode { get; set; }

    [Pattern(Regex = "EXTENDED")]
    public bool Extended { get; set; }

    [Pattern(Regex = @"(- ?(?:.+\])?([^-\[]+)(?:\[.+\])?)$", AlternateRegex = @"(([A-Za-z0-9]+))$")]
    public string? Group { get; set; }

    [Pattern(Regex = @"HDR(?:\s?10)?(?:[\s\.\]])")]
    public bool HDR { get; set; }

    [Pattern(Regex = "HC")]
    public bool HardCoded { get; set; }

    [Pattern(Regex = "([^A-Za-z0-9](3D)[^A-Za-z0-9])")]
    public bool Is3D { get; set; }

    [Pattern(Regex = "MULT[iI]-?(?:[0-9]+)?")]
    public bool MultipleLanguages { get; set; }

    public string? Name { get; set; }

    [Pattern(Regex = "NF")]
    public bool Netflix { get; set; }

    [Pattern(Regex = "PROPER")]
    public bool Proper { get; set; }

    [Pattern(Regex = @"(?:PPV\.)?[HP]DTV|(?:HD)?C[Aa][Mm]|B[DrR]R[iI][pP][sS]?|TS|(?:PPV )?WEB[- ]?DL(?: DVDRip)?|H[dD]Rip|DVDRip|DVDRiP|DVDRIP|CamRip|W[EB]B[rR]ip|[Bb]lu[ -]?[Rr]ay|DvDScr|hdtv|UHD(?: B[Ll][Uu][- ]?[Rr][Aa][Yy])")]
    public string? Quality { get; set; }

    [Pattern(Regex = @"R[0-9]")]
    public string? Region { get; set; }

    [Pattern(Regex = "REMASTERED")]
    public bool Remastered { get; set; }

    [Pattern(Regex = "REMUX")]
    public bool Remux { get; set; }

    [Pattern(Regex = "REPACK")]
    public bool Repack { get; set; }

    [Pattern(Regex = @"(([0-9]{3,4}p))[^M]")]
    public string? Resolution { get; set; }

    [Pattern(Regex = @"([Ss]([0-9]{1,2}))[Eex\s]")]
    public int? Season { get; set; }

    [Pattern(Regex = @"(?:full|half)[-\s](?:sbs|ou)", Options = RegexOptions.IgnoreCase)]
    public string? ThreeDFormat { get; set; }

    public string? Title { get; set; }

    [Pattern(Regex = @"TrueHD(?:[ \.]MA)?")]
    public bool TrueHD { get; set; }

    [Pattern(Regex = @"^(\[ ?([^\]]+?) ?\])")]
    public string? Website { get; set; }

    [Pattern(Regex = @"([\[\(]?((?:19|20)[0-9]{2})[\]\)]?)")]
    public int Year { get; set; }

    [Pattern(Regex = @"1400Mb|3rd Nov| ((Rip))| \[no rar\]|[\[\(]?[Rr][Ee][Qq][\]\)]?")]
    public string? Garbage { get; set; }

    public override string ToString() => Name;

    public static ParsedTorrent ParseInfo(string name)
    {
        var response = new ParsedTorrent();

        var end = name.Length;
        var start = 0;
        var clean = "";

        name = HttpUtility.HtmlDecode(name);

        if (!string.IsNullOrEmpty(name))
        {
            var props = response.GetType().GetProperties()
                .Where(c => c.GetCustomAttributes(false).Any(d => d is PatternAttribute));

            foreach (var prop in props)
            {
                var attribute =
                    (PatternAttribute) prop.GetCustomAttributes(false).First(c => c is PatternAttribute);
                var match = Regex.Match(name, attribute.Regex, attribute.Options);

                if (!match.Success && !string.IsNullOrEmpty(attribute.AlternateRegex))
                {
                    match = Regex.Match(name, attribute.AlternateRegex, attribute.Options);
                }

                if (match.Success)
                {
                    var cleanIndex = match.Groups.Count > 1 ? 2 : 0;
                    clean = match.Groups[cleanIndex].Value;

                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        prop.SetValue(response, int.Parse(clean));
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        prop.SetValue(response, true);
                    }
                    else
                    {
                        if (prop.Name == "Group")
                        {
                            clean = Regex.Replace(clean, @" *\([^)]*\) *", "");
                            clean = Regex.Replace(clean, @" *\[[^)]*\] *", "");
                        }

                        if (!string.IsNullOrEmpty(attribute.Replacements))
                        {
                            foreach (var replace in attribute.Replacements.Split('|'))
                            {
                                var parts = replace.Split(',');

                                if (parts.Length == 2)
                                {
                                    clean = clean.Replace(parts[0], parts[1]);
                                }
                            }
                        }

                        prop.SetValue(response, clean);
                    }

                    if (match.Index == 0)
                    {
                        start = match.Groups[0].Length;
                    }
                    else if (match.Index < end)
                    {
                        end = match.Index;
                    }
                }
            }

            var raw = name[start..end].Split('(')[0];
            clean = Regex.Replace(raw, @"^ -", "");

            if (clean.IndexOf(' ') == -1 && clean.IndexOf('.') != -1)
            {
                clean = Regex.Replace(clean, @"\.", " ");
            }

            clean = Regex.Replace(clean, @"_|\.", " ");
            clean = Regex.Replace(clean, @"([\(_]|- ?)$", "").Trim();
            response.Title = clean;
            response.Name = name;
        }

        return response;
    }
}
