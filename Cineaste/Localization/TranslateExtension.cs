namespace Cineaste.Localization;

public sealed class TranslateExtension : MarkupExtension
{
    public TranslateExtension(string key) =>
        this.Key = key;

    [ConstructorArgument("key")]
    public string Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        Messages.ResourceManager.GetString(this.Key) ?? this.Key;
}
