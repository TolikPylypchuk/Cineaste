namespace Cineaste.Client.FormModels;

public abstract class TitledFormModel<TModel, TRequest> : FormModel<TModel, TRequest>
    where TModel : ITitledModel
{
    public ObservableCollection<string> Titles { get; } = new();
    public ObservableCollection<string> OriginalTitles { get; } = new();

    public ImmutableList<TitleRequest> ToTitleRequests(IEnumerable<string> titles) =>
        titles.Select((title, index) => new TitleRequest(title, index + 1)).ToImmutableList();

    protected void CopyTitles(TModel? model)
    {
        this.Titles.Clear();

        foreach (var title in model?.Titles.OrderBy(t => t.Priority) ?? Enumerable.Empty<TitleModel>())
        {
            this.Titles.Add(title.Name);
        }

        this.OriginalTitles.Clear();

        foreach (var title in model?.OriginalTitles.OrderBy(t => t.Priority) ?? Enumerable.Empty<TitleModel>())
        {
            this.OriginalTitles.Add(title.Name);
        }
    }
}
