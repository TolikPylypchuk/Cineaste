namespace Cineaste.Client.ViewModels;

using System.Globalization;

public sealed class CreateListPageViewModel : ReactiveObject
{
    [Reactive]
    public string Name { get; set; } = String.Empty;

    public string Handle { [ObservableAsProperty] get; } = String.Empty;

    [Reactive]
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    public IReadOnlyCollection<CultureInfo> AllCultures { get; }

    public CreateListPageViewModel()
    {
        this.WhenAnyValue(vm => vm.Name)
            .Select(this.CreateHandleFromName)
            .ToPropertyEx(this, vm => vm.Handle);

        this.AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList().AsReadOnly();
    }

    private string CreateHandleFromName(string name) =>
        Uri.EscapeDataString(name.Trim()
            .Replace("&", "-and-")
            .Replace("@", "-at-")
            .Replace("/", String.Empty)
            .Replace("\\", String.Empty)
            .Replace(".", String.Empty)
            .Replace(",", String.Empty)
            .Replace("!", String.Empty)
            .Replace("?", String.Empty)
            .Replace("|", String.Empty)
            .Replace("#", String.Empty)
            .Replace("$", String.Empty)
            .Replace("^", String.Empty)
            .Replace("*", String.Empty)
            .Replace("(", String.Empty)
            .Replace(")", String.Empty)
            .Replace(" ", "-")
            .Replace("\t", "-")
            .Replace("\r\n", "-")
            .Replace("\n", "-")
            .Replace("--", "-"))
            .ToLowerInvariant();
}
