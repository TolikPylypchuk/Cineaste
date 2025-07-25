namespace Cineaste.Client.FormModels;

public abstract class TitledFormModelBase<TRequest, TModel> : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>, ITitledRequest
    where TModel : ITitledModel
{
    public IList<string> Titles { get; }
    public IList<string> OriginalTitles { get; }

    protected TitledFormModelBase()
    {
        this.Titles = new TitleList(this.OnTitlesUpdated);
        this.OriginalTitles = new TitleList(this.OnOriginalTitlesUpdated);
    }

    public ImmutableValueList<TitleRequest> ToTitleRequests(IEnumerable<string> titles) =>
        titles.Select((title, index) => new TitleRequest(title, index + 1)).ToImmutableList().AsValue();

    protected void CopyTitles(TModel? model, string defaultTitle = "", string defaultOriginalTitle = "")
    {
        this.Titles.Clear();

        foreach (var title in model?.Titles.OrderBy(t => t.SequenceNumber).Select(t => t.Name) ?? [defaultTitle])
        {
            this.Titles.Add(title);
        }

        this.OriginalTitles.Clear();

        foreach (var title in model?.OriginalTitles.OrderBy(t => t.SequenceNumber).Select(t => t.Name)
            ?? [defaultOriginalTitle])
        {
            this.OriginalTitles.Add(title);
        }
    }

    private void OnTitlesUpdated()
    {
        this.UpdateRequest();
        this.TitlesUpdated?.Invoke(this, EventArgs.Empty);
        this.OnPropertyChanged(nameof(this.Titles));
    }

    private void OnOriginalTitlesUpdated()
    {
        this.UpdateRequest();
        this.OriginalTitlesUpdated?.Invoke(this, EventArgs.Empty);
        this.OnPropertyChanged(nameof(this.Titles));
    }

    public event EventHandler? TitlesUpdated;
    public event EventHandler? OriginalTitlesUpdated;
}
