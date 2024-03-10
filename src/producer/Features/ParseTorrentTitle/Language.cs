namespace Producer.Features.ParseTorrentTitle;

public sealed class Language : SmartEnum<Language, string>
{
    public static readonly Language English = new("English", "English");
    public static readonly Language French = new("French", "French");
    public static readonly Language Spanish = new("Spanish", "Spanish");
    public static readonly Language German = new("German", "German");
    public static readonly Language Italian = new("Italian", "Italian");
    public static readonly Language Danish = new("Danish", "Danish");
    public static readonly Language Dutch = new("Dutch", "Dutch");
    public static readonly Language Japanese = new("Japanese", "Japanese");
    public static readonly Language Cantonese = new("Cantonese", "Cantonese");
    public static readonly Language Mandarin = new("Mandarin", "Mandarin");
    public static readonly Language Russian = new("Russian", "Russian");
    public static readonly Language Polish = new("Polish", "Polish");
    public static readonly Language Vietnamese = new("Vietnamese", "Vietnamese");
    public static readonly Language Nordic = new("Nordic", "Nordic");
    public static readonly Language Swedish = new("Swedish", "Swedish");
    public static readonly Language Norwegian = new("Norwegian", "Norwegian");
    public static readonly Language Finnish = new("Finnish", "Finnish");
    public static readonly Language Turkish = new("Turkish", "Turkish");
    public static readonly Language Portuguese = new("Portuguese", "Portuguese");
    public static readonly Language Flemish = new("Flemish", "Flemish");
    public static readonly Language Greek = new("Greek", "Greek");
    public static readonly Language Korean = new("Korean", "Korean");
    public static readonly Language Hungarian = new("Hungarian", "Hungarian");
    public static readonly Language Persian = new("Persian", "Persian");
    public static readonly Language Bengali = new("Bengali", "Bengali");
    public static readonly Language Bulgarian = new("Bulgarian", "Bulgarian");
    public static readonly Language Brazilian = new("Brazilian", "Brazilian");
    public static readonly Language Hebrew = new("Hebrew", "Hebrew");
    public static readonly Language Czech = new("Czech", "Czech");
    public static readonly Language Ukrainian = new("Ukrainian", "Ukrainian");
    public static readonly Language Catalan = new("Catalan", "Catalan");
    public static readonly Language Chinese = new("Chinese", "Chinese");
    public static readonly Language Thai = new("Thai", "Thai");
    public static readonly Language Hindi = new("Hindi", "Hindi");
    public static readonly Language Tamil = new("Tamil", "Tamil");
    public static readonly Language Arabic = new("Arabic", "Arabic");
    public static readonly Language Estonian = new("Estonian", "Estonian");
    public static readonly Language Icelandic = new("Icelandic", "Icelandic");
    public static readonly Language Latvian = new("Latvian", "Latvian");
    public static readonly Language Lithuanian = new("Lithuanian", "Lithuanian");
    public static readonly Language Romanian = new("Romanian", "Romanian");
    public static readonly Language Slovak = new("Slovak", "Slovak");
    public static readonly Language Serbian = new("Serbian", "Serbian");

    private Language(string name, string value) : base(name, value) { }
}