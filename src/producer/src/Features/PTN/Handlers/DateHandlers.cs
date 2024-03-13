namespace Producer.Features.PTN.Handlers;

public class DateHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W|^)([([]?(?:19[6-9]|20[012])[0-9]([. \-/\\])(?:0[1-9]|1[012])\2(?:0[1-9]|[12][0-9]|3[01])[)\]]?)(?=\W|$)"), Transformers.Date("YYYY MM DD"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W|^)([([]?(?:0[1-9]|[12][0-9]|3[01])([. \-/\\])(?:0[1-9]|1[012])\2(?:19[6-9]|20[012])[0-9][)\]]?)(?=\W|$)"), Transformers.Date("DD MM YYYY"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W)([([]?(?:0[1-9]|1[012])([. \-/\\])(?:0[1-9]|[12][0-9]|3[01])\2(?:[0][1-9]|[0126789][0-9])[)\]]?)(?=\W|$)"), Transformers.Date("MM DD YY"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W)([([]?(?:0[1-9]|[12][0-9]|3[01])([. \-/\\])(?:0[1-9]|1[012])\2(?:[0][1-9]|[0126789][0-9])[)\]]?)(?=\W|$)"), Transformers.Date("DD MM YY"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W|^)([([]?(?:0?[1-9]|[12][0-9]|3[01])[. ]?(?:st|nd|rd|th)?([. \-/\\])(?:feb(?:ruary)?|jan(?:uary)?|mar(?:ch)?|apr(?:il)?|may|june?|july?|aug(?:ust)?|sept?(?:ember)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)\2(?:19[7-9]|20[012])[0-9][)\]]?)(?=\W|$)", RegexOptions.IgnoreCase), Transformers.Date("DD MMM YYYY"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W|^)([([]?(?:0?[1-9]|[12][0-9]|3[01])[. ]?(?:st|nd|rd|th)?([. \-/\\])(?:feb(?:ruary)?|jan(?:uary)?|mar(?:ch)?|apr(?:il)?|may|june?|july?|aug(?:ust)?|sept?(?:ember)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)\2(?:0[1-9]|[0126789][0-9])[)\]]?)(?=\W|$)", RegexOptions.IgnoreCase), Transformers.Date("DD MMM YY"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Date, new Regex(@"(?<=\W|^)([([]?20[012][0-9](?:0[1-9]|1[012])(?:0[1-9]|[12][0-9]|3[01])[)\]]?)(?=\W|$)"), Transformers.Date("YYYYMMDD"), new() { Remove = true });
    }
}