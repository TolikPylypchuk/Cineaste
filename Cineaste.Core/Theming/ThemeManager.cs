namespace Cineaste.Core.Theming;

public sealed class ThemeManager : ReactiveObject
{
    public ThemeManager(Theme initialTheme) =>
        this.Theme = initialTheme;

    [Reactive]
    public Theme Theme { get; set; }

    public override string ToString() =>
        $"Current theme: {Enum.GetName(this.Theme)}";
}
