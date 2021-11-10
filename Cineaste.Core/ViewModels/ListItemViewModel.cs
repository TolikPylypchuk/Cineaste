namespace Cineaste.Core.ViewModels;

public sealed class ListItemViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable themeSubscription;

    public ListItemViewModel(
        ListItem item,
        IThemeAwareColorGenerator? generator = null)
    {
        this.Item = item;
        this.Color = item.Color;

        generator ??= GetDefaultService<IThemeAwareColorGenerator>();

        this.themeSubscription = generator.CreateThemeAwareColor(this.Color)
            .BindTo(this, vm => vm.Color);
    }

    public ListItem Item { get; }

    [Reactive]
    public string Color { get; set; }

    public void Dispose() =>
        this.themeSubscription.Dispose();

    public override string ToString() =>
        $"Item: {this.Item}";
}
