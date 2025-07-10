namespace Cineaste.Client.Store.Forms;

public abstract record FormState<TModel>
{
    public TModel? Model { get; init; }

    public ApiCall Fetch { get; init; } = ApiCall.NotStarted();
    public ApiCall Create { get; init; } = ApiCall.NotStarted();
    public ApiCall Update { get; init; } = ApiCall.NotStarted();
    public ApiCall Delete { get; init; } = ApiCall.NotStarted();
}
