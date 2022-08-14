namespace Cineaste.Client.FormModels;

public abstract class TitledFormModel<TModel> : FormModel<TModel>
    where TModel : ITitledModel
{
    public ObservableCollection<string> Titles { get; } = new() { String.Empty };
    public ObservableCollection<string> OriginalTitles { get; } = new() { String.Empty };

    public ImmutableList<TitleRequest> ToTitleRequests(IEnumerable<string> titles) =>
        titles.Select((title, index) => new TitleRequest(title, index + 1)).ToImmutableList();

    protected void CopyTitles(TModel? model, string defaultTitle = "", string defaultOriginalTitle = "")
    {
        this.Titles.Clear();

        foreach (var title in model?.Titles.OrderBy(t => t.Priority).Select(t => t.Name) ?? new[] { defaultTitle })
        {
            this.Titles.Add(title);
        }

        this.OriginalTitles.Clear();

        foreach (var title in model?.OriginalTitles.OrderBy(t => t.Priority).Select(t => t.Name)
            ?? new[] { defaultOriginalTitle })
        {
            this.OriginalTitles.Add(title);
        }
    }
}
