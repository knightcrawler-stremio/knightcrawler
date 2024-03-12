namespace Tissue.Features.Wordlists;

public interface IWordCollections
{
    HashSet<string> AdultWords { get; }

    HashSet<string> AdultCompoundPhrases { get; }

    HashSet<string> Jav { get; }

    HashSet<string> AdultStars { get; }

    Task LoadAsync();
}
