using Cineaste.Client.Components.Forms;

namespace Cineaste.Client.Components.Base;

public abstract class PosterCineasteForm<TFormModel, TRequest, TModel, TState>
    : CineasteForm<TFormModel, TRequest, TModel, TState>
    where TFormModel : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>
    where TModel : IIdentifyableModel
{
    [Inject]
    public required IDialogService DialogService { get; init; }

    private Dictionary<Guid, PosterRequest> PosterRequests { get; } = [];

    protected async Task OpenPosterDialog(string title, Guid id = default)
    {
        var parameters = new DialogParameters
        {
            [nameof(PosterDialog.ItemTitle)] = title
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            BackdropClick = false,
            NoHeader = true
        };

        var dialog = await this.DialogService.ShowAsync<PosterDialog>(null, parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is PosterRequest request &&
            this.CreateSetPosterAction(id, request) is { } action)
        {
            this.PosterRequests[id] = request;
            this.Dispatcher.Dispatch(action);
        }
    }

    protected async Task RemovePoster(string title, string body, Guid id = default)
    {
        bool? delete = await this.DialogService.ShowMessageBox(
            title: this.Loc[title],
            markupMessage: new MarkupString(this.Loc[body]),
            yesText: this.Loc["Confirmation.Confirm"],
            noText: this.Loc["Confirmation.Cancel"]);

        if (delete == true && this.CreateRemovePosterAction(id) is { } action)
        {
            this.Dispatcher.Dispatch(action);
        }
    }

    protected void OnPosterUpdated() =>
        this.OnPosterUpdated(default);

    protected void OnPosterUpdated(Guid id)
    {
        this.UpdateFormModel();
        this.PosterRequests.Remove(id);
    }

    protected void SetPoster() =>
        this.SetPoster(default);

    protected void SetPoster(Guid id)
    {
        if (this.PosterRequests.TryGetValue(id, out var request) &&
            this.CreateSetPosterAction(id, request) is { } action)
        {
            this.Dispatcher.Dispatch(action);
        }
    }

    protected abstract object? CreateSetPosterAction(Guid id, PosterRequest request);

    protected abstract object? CreateRemovePosterAction(Guid id);

    protected abstract void UpdateFormModel();
}
