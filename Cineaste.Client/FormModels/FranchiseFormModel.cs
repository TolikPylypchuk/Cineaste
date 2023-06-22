namespace Cineaste.Client.FormModels;

public sealed class FranchiseFormModel : TitledFormModelBase<FranchiseRequest, FranchiseModel>
{
    private readonly Guid listId;
    private readonly ObservableCollection<FranchiseFormComponent> components = new();

    public ReadOnlyObservableCollection<FranchiseFormComponent> Components { get; }

    public bool ShowTitles { get; set; }

    public bool IsLooselyConnected { get; set; }

    public bool ContinueNumbering { get; set; }

    public FranchiseFormModel(Guid listId)
    {
        this.listId = listId;
        this.Components = new(this.components);
        this.FinishInitialization();
    }

    public override FranchiseRequest CreateRequest() =>
        new(
            this.listId,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.components
                .Select(c => new FranchiseItemRequest(c.Id, c.Type, c.SequenceNumber, c.ShouldDisplayNumber))
                .ToImmutableList()
                .AsValue(),
            this.ShowTitles,
            this.IsLooselyConnected,
            this.ContinueNumbering);

    protected override void CopyFromModel()
    {
        var franchise = this.BackingModel;

        this.CopyTitles(franchise);

        this.ShowTitles = franchise?.ShowTitles ?? false;
        this.IsLooselyConnected = franchise?.IsLooselyConnected ?? false;
        this.ContinueNumbering = franchise?.ContinueNumbering ?? false;

        this.components.Clear();

        if (franchise is not null)
        {
            foreach (var item in franchise.Items)
            {
                this.components.Add(new FranchiseFormComponent(
                    item.Id, item.Type, item.Title, item.SequenceNumber, item.ShouldDisplayNumber));
            }
        }
    }
}
