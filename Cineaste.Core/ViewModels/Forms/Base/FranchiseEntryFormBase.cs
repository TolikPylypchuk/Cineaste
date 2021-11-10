namespace Cineaste.Core.ViewModels.Forms.Base;

public abstract class FranchiseEntryFormBase<TModel, TForm> : TitledFormBase<TModel, TForm>, IFranchiseEntryForm
    where TModel : class
    where TForm : FranchiseEntryFormBase<TModel, TForm>
{
    private readonly BehaviorSubject<bool> canCreateFranchiseSubject = new(false);

    protected FranchiseEntryFormBase(
        FranchiseEntry? entry,
        ResourceManager? resourceManager,
        IScheduler? scheduler = null)
        : base(resourceManager, scheduler)
    {
        this.FranchiseEntry = entry;

        var formNotChanged = this.FormChanged.Invert();

        int lastSequenceNumber = this.FranchiseEntry?.ParentFranchise
            .Entries
            .Select(e => (int?)e.SequenceNumber)
            .Max()
            ?? 0;

        var canGoToFranchise = this.IfFranchisePresent(() => formNotChanged);

        this.GoToFranchise = ReactiveCommand.Create<Unit, Franchise>(
            _ => this.FranchiseEntry!.ParentFranchise, canGoToFranchise);

        var canGoToNext = this.IfFranchisePresent(() =>
            this.FranchiseEntry!.SequenceNumber >= lastSequenceNumber
                ? Observable.Return(false)
                : formNotChanged);

        this.GoToNext = ReactiveCommand.Create<Unit, FranchiseEntry>(
            _ => this.FranchiseEntry!.ParentFranchise.Entries
                .OrderBy(e => e.SequenceNumber)
                .First(e => e.SequenceNumber > this.FranchiseEntry!.SequenceNumber),
            canGoToNext);

        var canGoToPrevious = this.IfFranchisePresent(() =>
            this.FranchiseEntry!.SequenceNumber == 1
                ? Observable.Return(false)
                : formNotChanged);

        this.GoToPrevious = ReactiveCommand.Create<Unit, FranchiseEntry>(
            _ => this.FranchiseEntry!.ParentFranchise.Entries
                .OrderBy(e => e.SequenceNumber)
                .Last(e => e.SequenceNumber < this.FranchiseEntry!.SequenceNumber),
            canGoToPrevious);

        this.CreateFranchise = ReactiveCommand.Create(() => { }, this.canCreateFranchiseSubject);
    }

    public FranchiseEntry? FranchiseEntry { get; }

    public ReactiveCommand<Unit, Franchise> GoToFranchise { get; }
    public ReactiveCommand<Unit, FranchiseEntry> GoToNext { get; }
    public ReactiveCommand<Unit, FranchiseEntry> GoToPrevious { get; }
    public ReactiveCommand<Unit, Unit> CreateFranchise { get; }

    protected void CanCreateFranchise() =>
        Observable.CombineLatest(
               Observable.Return(!this.IsNew).Merge(this.Save.Select(_ => true)),
               Observable.Return(this.FranchiseEntry == null),
               this.FormChanged.Invert())
            .AllTrue()
            .Subscribe(this.canCreateFranchiseSubject);

    private IObservable<bool> IfFranchisePresent(Func<IObservable<bool>> observableProvider) =>
        this.FranchiseEntry == null
            ? Observable.Return(false)
            : observableProvider();
}
