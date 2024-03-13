namespace Producer.Features.PTN.Handlers;

public class LanguagesHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bmulti(?:ple)?[ .-]*(?:su?$|sub\w*|dub\w*)\b|msub", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("multi subs")), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bmulti(?:ple)?[ .-]*(?:lang(?:uages?)?|audio|VF2)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("multi audio")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\btri(?:ple)?[ .-]*(?:audio|dub\w*)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("multi audio")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bdual[ .-]*(?:au?$|[aá]udio|line)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("dual audio")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bdual\b(?![ .-]*sub)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("dual audio")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bengl?(?:sub[A-Z]*)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\beng?sub[A-Z]*\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bing(?:l[eéê]s)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\benglish\W+(?:subs?|sdh|hi)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bEN\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\benglish?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("english")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:JP|JAP|JPN)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("japanese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(japanese|japon[eê]s)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("japanese")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:KOR|kor[ .-]?sub)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("korean")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(korean|coreano)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("korean")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:traditional\W*chinese|chinese\W*traditional)(?:\Wchi)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("taiwanese")), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bzh-hant\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("taiwanese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:mand[ae]rin|ch[sn])\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("chinese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bCH[IT]\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("chinese")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(chinese|chin[eê]s|chi)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("chinese")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bzh-hans\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("chinese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bFR(?:ench|a|e|anc[eê]s)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("french")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(Truefrench|VF[FI])\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("french")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(VOST(?:FR?|A)?|SUBFRENCH)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("french")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bspanish\W?latin|american\W*(?:spa|esp?)/i", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("latino")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:audio.)?lat(?:i|ino)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("latino")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:audio.)?(?:ESP|spa|(en[ .]+)?espa[nñ]ola?|castellano)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bes(?=[ .,/-]+(?:[A-Z]{2}[ .,/-]+){2,})\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<=[ .,/-]+(?:[A-Z]{2}[ .,/-]+){2,})es\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<=[ .,/-]+[A-Z]{2}[ .,/-]+)es(?=[ .,/-]+[A-Z]{2}[ .,/-]+)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bes(?=\.(?:ass|ssa|srt|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bspanish\W+subs?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(spanish|espanhol)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("spanish")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:p[rt]|en|port)[. (\\/-]*BR\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bbr(?:a|azil|azilian)\W+(?:pt|por)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:leg(?:endado|endas?)?|dub(?:lado)?|portugu[eèê]se?)[. -]*BR\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bleg(?:endado|endas?)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bportugu[eèê]s[ea]?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bPT[. -]*(?:PT|ENG?|sub(?:s|titles?))\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bpt(?=\.(?:ass|ssa|srt|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bpor\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("portuguese")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bITA\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("italian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<!w{3}\.\w+\.)IT(?=[ .,/-]+(?:[a-zA-Z]{2}[ .,/-]+){2,})\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("italian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bit(?=\.(?:ass|ssa|srt|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("italian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bitaliano?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("italian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bgreek[ .-]*(?:audio|lang(?:uage)?|subs?(?:titles?)?)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("greek")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:GER|DEU)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bde(?=[ .,/-]+(?:[A-Z]{2}[ .,/-]+){2,})\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<=[ .,/-]+(?:[A-Z]{2}[ .,/-]+){2,})de\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<=[ .,/-]+[A-Z]{2}[ .,/-]+)de(?=[ .,/-]+[A-Z]{2}[ .,/-]+)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bde(?=\.(?:ass|ssa|srt|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(german|alem[aã]o)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("german")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bRUS?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("russian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(russian|russo)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("russian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bUKR\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("ukrainian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bukrainian\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("ukrainian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bhin(?:di)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("hindi")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:(?<!w{3}\.\w+\.)tel(?!\W*aviv)|telugu)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("telugu")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bt[aâ]m(?:il)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("tamil")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?<!YTS\.)LT\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("lithuanian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\blithuanian\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("lithuanian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\blatvian\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("latvian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bestonian\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("estonian")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:(?<!w{3}\.\w+\.)PL|pol)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("polish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(polish|polon[eê]s|polaco)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("polish")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bCZ[EH]?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("czech")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bczech\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("czech")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bslo(?:vak|vakian|subs|[\]_)]?\.\w{2,4}$)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("slovakian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bHU\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("hungarian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bHUN(?:garian)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("hungarian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bROM(?:anian)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("romanian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bRO(?=[ .,/-]*(?:[A-Z]{2}[ .,/-]+)*sub)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("romanian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bbul(?:garian)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("bulgarian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:srp|serbian)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("serbian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:HRV|croatian)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("croatian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bHR(?=[ .,/-]*(?:[A-Z]{2}[ .,/-]+)*sub)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("croatian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bslovenian\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("slovenian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:(?<!w{3}\.\w+\.)NL|dut|holand[eê]s)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("dutch")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bdutch\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("dutch")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bflemish\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("dutch")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:DK|danska|dansub|nordic)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("danish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(danish|dinamarqu[eê]s)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("danish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bdan\b(?=.*\.(?:srt|vtt|ssa|ass|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("danish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:(?<!w{3}\.\w+\.)FI|finsk|finsub|nordic)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("finnish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bfinnish\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("finnish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:(?<!w{3}\.\w+\.)SE|swe|swesubs?|sv(?:ensk)?|nordic)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("swedish")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(swedish|sueco)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("swedish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:NOR|norsk|norsub|nordic)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("norwegian")), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(norwegian|noruegu[eê]s|bokm[aå]l|nob|nor(?=[\]_)]?\.\w{2,4}$))\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("norwegian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:arabic|[aá]rabe|ara)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("arabic")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\barab.*(?:audio|lang(?:uage)?|sub(?:s|titles?)?)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("arabic")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bar(?=\.(?:ass|ssa|srt|sub|idx)$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("arabic")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:turkish|tur(?:co)?)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("turkish")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bvietnamese\b|\bvie(?=[\]_)]?\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("vietnamese")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bind(?:onesian)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("indonesian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(thai|tailand[eê]s)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("thai")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(THA|tha)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("thai")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(?:malay|may(?=[\]_)]?\.\w{2,4}$)|(?<=subs?\([a-z,]+)may)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("malay")), new() { SkipIfFirst = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\bheb(?:rew|raico)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("hebrew")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, new Regex(@"\b(persian|persa)\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("persian")), new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Languages, Handler.CreateHandlerFromDelegate(ResultKeys.Languages, DistinguishPortgueseFromSpanish), Transformers.None, new() { SkipFromTitle = true, SkipIfAlreadyFound = false });
    }
    
    private static Func<Dictionary<string, object>, HandlerResult> DistinguishPortgueseFromSpanish
        => context =>
        {
            var title = context[ResultKeys.Title] as string;
            var result = context[ResultKeys.Result] as Dictionary<string, object>;
            var matched = context[ResultKeys.Matched] as Dictionary<string, object>;

            if (result.TryGetValue(ResultKeys.Languages, out var value))
            {
                if (value is not List<string> languages || SpanishPortugese.All(l => !languages.Contains(l)))
                {
                    var episodes = matched?[ResultKeys.Episodes] as Dictionary<string, object>;
                    var rawMatch = episodes?["rawMatch"] as string;

                    if ((rawMatch != null && Regex.IsMatch(rawMatch, "capitulo|ao", RegexOptions.IgnoreCase)) || 
                        (title != null && Regex.IsMatch(title, "dublado", RegexOptions.IgnoreCase)))
                    {
                        result[ResultKeys.Languages] = (result[ResultKeys.Languages] as List<string> ?? []).Concat(new List<string> { "portuguese" }).ToList();
                    }
                }
            }

            return new HandlerResult { MatchIndex = 0 };
        };

    private static readonly string[] SpanishPortugese = ["portuguese", "spanish"];
}