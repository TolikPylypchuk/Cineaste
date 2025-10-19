namespace Cineaste.Client.Services.Navigation;

public sealed class PageNavigator(NavigationManager navigationManager) : IPageNavigator
{
    private const string LandingPage = "/";
    private const string LoginPage = "/login";
    private const string RegisterPage = "/register";
    private const string ListPage = "/list";
    private const string ListSettingsPage = "/list/settings";
    private const string ProfilePage = "/profile";

    public string GetPageUrl(CineastePage page) =>
        page switch
        {
            CineastePage.Landing => LandingPage,
            CineastePage.Login => LoginPage,
            CineastePage.Register => RegisterPage,
            CineastePage.List => ListPage,
            CineastePage.Profile => ProfilePage,
            _ => LandingPage
        };

    public void GoToPage(CineastePage page, bool forceReload = false)
    {
        switch (page)
        {
            case CineastePage.Landing:
                this.GoTo(LandingPage, forceReload);
                break;
            case CineastePage.Login:
                this.GoToLoginPage(forceReload);
                break;
            case CineastePage.Register:
                this.GoTo(RegisterPage, forceReload);
                break;
            case CineastePage.List:
                this.GoTo(ListPage, forceReload);
                break;
            case CineastePage.ListSettings:
                this.GoTo(ListSettingsPage, forceReload);
                break;
            case CineastePage.Profile:
                this.GoTo(ProfilePage, forceReload);
                break;
        }
    }

    public void GoToPage(string? url, bool forceReload = false)
    {
        if (String.IsNullOrEmpty(url))
        {
            this.GoTo(LandingPage, forceReload);
        } else
        {
            this.GoTo(url, forceReload);
        }
    }

    private void GoToLoginPage(bool forceReload)
    {
        var returnUrl = new Uri(navigationManager.Uri, UriKind.Absolute)
            .MakeRelativeUri(new Uri(navigationManager.BaseUri, UriKind.Absolute))
            .ToString();

        this.GoTo(
            String.IsNullOrEmpty(returnUrl) || returnUrl == "/"
                ? LoginPage
                : navigationManager.GetUriWithQueryParameters(
                    LoginPage, new Dictionary<string, object?> { ["returnUrl"] = returnUrl }),
            forceReload);
    }

    private void GoTo(string url, bool forceReload) =>
        navigationManager.NavigateTo(url, forceReload);
}
