namespace Cineaste.Core.Preferences;

[ToString]
[Equals(DoNotAddEqualityOperators = true)]
public sealed class UIPreferences
{
    public UIPreferences(Theme theme) =>
        this.Theme = theme;

    public Theme Theme { get; set; }
}
