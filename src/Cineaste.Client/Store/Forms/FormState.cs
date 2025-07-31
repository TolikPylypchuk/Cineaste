namespace Cineaste.Client.Store.Forms;

public abstract record FormState<TModel>
{
    public TModel? Model { get; init; }

    public Guid? InitialParentFranchiseId { get; init; }

    public ApiCall Fetch { get; init; } = ApiCall.NotStarted();
    public ApiCall Add { get; init; } = ApiCall.NotStarted();
    public ApiCall Update { get; init; } = ApiCall.NotStarted();
    public ApiCall Remove { get; init; } = ApiCall.NotStarted();
}
