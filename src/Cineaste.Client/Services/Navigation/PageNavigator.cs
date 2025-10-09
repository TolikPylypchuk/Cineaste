namespace Cineaste.Client.Services.Navigation;

public sealed class PageNavigator(NavigationManager navigationManager) : IPageNavigator
{
    private readonly NavigationManager navigationManager = navigationManager;

    public string HomePage => "/";
    public string LoginPage => "/login";
    public string RegisterPage => "/register";
    public string ListPage => "/list";
    public string ListSettingsPage => "/list/settings";

    public void GoToHomePage() =>
        this.GoTo(this.HomePage);

    public void GoToLoginPage(bool forceReload = false)
    {
        var returnUrl = this.ToUri(this.navigationManager.Uri)
            .MakeRelativeUri(this.ToUri(this.navigationManager.BaseUri))
            .ToString();

        this.GoTo(
            String.IsNullOrEmpty(returnUrl) || returnUrl == "/"
                ? this.LoginPage
                : $"{this.LoginPage}?returnUrl={Uri.EscapeDataString(returnUrl)}",
            forceReload);
    }

    public void GoToRegsiterPage() =>
        this.GoTo(this.RegisterPage);

    public void GoToListPage() =>
        this.GoTo(this.ListPage);

    public void GoToListSettingsPage() =>
        this.GoTo(this.ListSettingsPage);

    public void GoToPage(string? url)
    {
        if (!String.IsNullOrEmpty(url))
        {
            this.navigationManager.NavigateTo(url);
        } else
        {
            this.GoToHomePage();
        }
    }

    private void GoTo(string url, bool forceLoad = false) =>
        this.navigationManager.NavigateTo(url, forceLoad);

    private Uri ToUri(string uri) =>
        new(uri, UriKind.Absolute);
}
