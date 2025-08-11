namespace Cineaste.Client.Store.Forms;

public abstract record PosterFormState<TModel> : FormState<TModel>
{
    public ApiCall SetPoster { get; init; } = ApiCall.NotStarted();
    public ApiCall RemovePoster { get; init; } = ApiCall.NotStarted();
}
