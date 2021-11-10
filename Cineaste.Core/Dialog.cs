namespace Cineaste.Core;

public static class Dialog
{
    public static readonly Interaction<MessageModel, Unit> ShowMessage = new();
    public static readonly Interaction<ConfirmationModel, bool> Confirm = new();
    public static readonly Interaction<InputModel, string?> Input = new();
    public static readonly Interaction<TagFormViewModel, Unit> TagForm = new();
    public static readonly Interaction<string, string?> SaveFile = new();
    public static readonly Interaction<Unit, string?> OpenFile = new();
    public static readonly Interaction<AboutModel, Unit> ShowAbout = new();

    public static IObservable<T?> PromptToDelete<T>(string messageAndTitle, Func<IObservable<T>> onDelete)
        where T : class =>
        Confirm.Handle(new ConfirmationModel(messageAndTitle))
            .SelectMany(shouldDelete => shouldDelete ? onDelete() : Observable.Return<T?>(null));
}
