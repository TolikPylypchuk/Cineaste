namespace Cineaste.Core.DialogModels;

public abstract class DialogModelBase<T> : ReactiveObject
{
    protected DialogModelBase(string message, string title)
    {
        this.Message = message;
        this.Title = title;

        this.Close = ReactiveCommand.Create<T, T>(result => result);
    }

    [Reactive]
    public string Message { get; set; }

    [Reactive]
    public string Title { get; set; }

    public ReactiveCommand<T, T> Close { get; }
}
