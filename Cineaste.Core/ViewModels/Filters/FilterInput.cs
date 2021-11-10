namespace Cineaste.Core.ViewModels.Filters;

public abstract class FilterInput : ReactiveObject
{
    protected Subject<Unit> inputChanged = new();

    private protected FilterInput()
    { }

    [Reactive]
    public string Description { get; set; } = String.Empty;

    public IObservable<Unit> InputChanged =>
        this.inputChanged.AsObservable();
}
