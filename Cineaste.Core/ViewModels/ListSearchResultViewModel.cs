namespace Cineaste.Core.ViewModels;

public sealed class ListSearchResultViewModel : ReactiveObject
{
    public ListSearchResultViewModel(ListItemViewModel item)
    {
        this.Item = item;
        this.Title = item.Item.Title;
        this.OriginalTitle = item.Item.OriginalTitle;
        this.Year = item.Item.Year;
        this.Tag = item.Item is MovieListItem ? nameof(Movie) : nameof(Series);
    }

    public ListItemViewModel Item { get; }

    public string Title { get; }
    public string OriginalTitle { get; }
    public string Year { get; }
    public string Tag { get; }
}
