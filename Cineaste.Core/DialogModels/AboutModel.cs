namespace Cineaste.Core.DialogModels;

public sealed class AboutModel : ReactiveObject
{
    public AboutModel() =>
        this.Close = ReactiveCommand.Create(() => { });

    [Reactive]
    public string Version { get; set; } = String.Empty;

    public ReactiveCommand<Unit, Unit> Close { get; }
}
