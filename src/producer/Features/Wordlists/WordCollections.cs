namespace Producer.Features.Wordlists;

public class WordCollections : IWordCollections
{
    private const string AdultWordsFile = "adult-words.txt";
    private const string AdultCompoundPhrasesFile = "adult-compound-words.txt";
    private const string CommonWordsFile = "common-words.txt";

    public HashSet<string> AdultWords { get; private set; } = [];
    public HashSet<string> AdultCompoundPhrases { get; private set; } = [];
    public HashSet<string> CommonWords { get; private set; } = [];

    public async Task LoadAsync()
    {
        var loaderTasks = new List<Task>
        {
            LoadAdultWords(),
            LoadAdultCompounds(),
            LoadCommonWords()
        };

        await Task.WhenAll(loaderTasks);
    }

    private async Task LoadCommonWords()
    {
        var commonWords = await File.ReadAllLinesAsync(GetPath(CommonWordsFile));
        CommonWords = [..commonWords];
    }

    private async Task LoadAdultCompounds()
    {
        var adultCompoundWords = await File.ReadAllLinesAsync(GetPath(AdultCompoundPhrasesFile));
        AdultCompoundPhrases = [..adultCompoundWords];
    }

    private async Task LoadAdultWords()
    {
        var adultWords = await File.ReadAllLinesAsync(GetPath(AdultWordsFile));
        AdultWords = [..adultWords];
    }

    private static string GetPath(string fileName) => Path.Combine(AppContext.BaseDirectory, "Data", fileName);
}
