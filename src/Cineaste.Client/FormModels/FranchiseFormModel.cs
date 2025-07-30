namespace Cineaste.Client.FormModels;

public sealed class FranchiseFormModel : TitledFormModelBase<FranchiseRequest, FranchiseModel>
{
    private readonly ObservableCollection<FranchiseFormComponent> components = [];
    private readonly ListKindModel defaultKind;

    public ReadOnlyObservableCollection<FranchiseFormComponent> Components { get; }

    public ListKindModel Kind { get; set; }
    public FranchiseKindSource KindSource { get; set; }

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool ContinueNumbering { get; set; }

    public Guid? ParentFranchiseId { get; private set; }

    public int? SequenceNumber { get; private set; }

    public bool IsFirst { get; private set; }
    public bool IsLast { get; private set; }

    public FranchiseFormModel(ListKindModel kind, FranchiseKindSource kindSource)
    {
        this.defaultKind = kind;
        this.Kind = kind;
        this.KindSource = kindSource;
        this.Components = new(this.components);
        this.FinishInitialization();
    }

    public override FranchiseRequest CreateRequest() =>
        new(
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.components
                .Select(c => new FranchiseItemRequest(c.Id, c.Type, c.SequenceNumber, c.DisplayNumber is not null))
                .ToImmutableList()
                .AsValue(),
            this.Kind.Id,
            this.KindSource,
            this.ShowTitles,
            this.IsLooselyConnected,
            this.ContinueNumbering);

    public void MoveComponentUp(FranchiseFormComponent component)
    {
        if (component.SequenceNumber == 1)
        {
            return;
        }

        int index = component.SequenceNumber - 1;

        var previousComponent = this.components[index - 1];

        this.components[index - 1] = component with
        {
            SequenceNumber = component.SequenceNumber - 1,
            DisplayNumber = this.Adjust(component.DisplayNumber, n => n - 1)
        };

        this.components[index] = previousComponent with
        {
            SequenceNumber = previousComponent.SequenceNumber + 1,
            DisplayNumber = this.Adjust(previousComponent.DisplayNumber, n => n + 1)
        };

        this.UpdateRequest();
    }

    public void MoveComponentDown(FranchiseFormComponent component)
    {
        if (component.SequenceNumber == this.components.Count)
        {
            return;
        }

        int index = component.SequenceNumber - 1;

        var nextComponent = this.components[index + 1];

        this.components[index + 1] = component with
        {
            SequenceNumber = component.SequenceNumber + 1,
            DisplayNumber = this.Adjust(component.DisplayNumber, n => n + 1)
        };

        this.components[index] = nextComponent with
        {
            SequenceNumber = nextComponent.SequenceNumber - 1,
            DisplayNumber = this.Adjust(nextComponent.DisplayNumber, n => n - 1)
        };

        this.UpdateRequest();
    }

    public void DetachComponent(FranchiseFormComponent component)
    {
        foreach (var nextComponent in this.components.Where(c => c.SequenceNumber > component.SequenceNumber).ToList())
        {
            this.components[nextComponent.SequenceNumber - 1] = nextComponent with
            {
                SequenceNumber = nextComponent.SequenceNumber - 1,
                DisplayNumber = this.Adjust(nextComponent.DisplayNumber, n => n - 1)
            };
        }

        this.components.Remove(component);

        if (this.components.Count == 0)
        {
            this.ShowTitles = true;
        }

        this.UpdateRequest();
    }

    protected override void CopyFromModel()
    {
        var franchise = this.BackingModel;

        this.CopyTitles(franchise);

        this.Kind = franchise?.Kind ?? this.defaultKind;
        this.KindSource = franchise?.KindSource ?? FranchiseKindSource.Movie;

        this.ShowTitles = franchise?.ShowTitles ?? false;
        this.IsLooselyConnected = franchise?.IsLooselyConnected ?? false;
        this.ContinueNumbering = franchise?.ContinueNumbering ?? false;

        this.components.Clear();

        if (franchise is not null)
        {
            foreach (var item in franchise.Items)
            {
                this.components.Add(new FranchiseFormComponent(
                    item.Id,
                    item.Type,
                    item.Title,
                    item.StartYear,
                    item.EndYear,
                    item.SequenceNumber,
                    item.DisplayNumber));
            }
        }

        this.ParentFranchiseId = franchise?.FranchiseItem?.ParentFranchiseId;
        this.SequenceNumber = franchise?.FranchiseItem?.SequenceNumber;
        this.IsFirst = franchise?.FranchiseItem?.IsFirstInFranchise ?? false;
        this.IsLast = franchise?.FranchiseItem?.IsLastInFranchise ?? false;
    }

    private int? Adjust(int? number, Func<int, int> adjuster) =>
        number is int num ? adjuster(num) : null;
}
