namespace Cineaste.Core.ViewModels.Forms;

public sealed class FranchiseAddableItemViewModel : ReactiveObject
{
    public FranchiseAddableItemViewModel(FranchiseEntry entry)
    {
        this.Entry = entry;
        this.Title = entry.GetTitle()?.Name ?? String.Empty;
        this.OriginalTitle = entry.GetOriginalTitle()?.Name ?? String.Empty;
        this.Year = entry.GetYears();
        this.Tag = entry.Movie != null
            ? nameof(Movie)
            : entry.Series != null ? nameof(Series) : nameof(Franchise);
    }

    public FranchiseEntry Entry { get; }

    public string Title { get; }
    public string OriginalTitle { get; }
    public string Year { get; }
    public string Tag { get; }
}
