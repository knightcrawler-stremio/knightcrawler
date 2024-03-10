namespace Producer.Features.Wordlists;

public interface IWordCollections
{
    HashSet<string> AdultWords { get; }

    HashSet<string> AdultCompoundPhrases { get; }

    HashSet<string> CommonWords { get; }

    Task LoadAsync();
}
