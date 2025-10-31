using System.Web;

using static Cineaste.Client.Navigation.NavigationPages;

namespace Cineaste.Client.Navigation;

public sealed class PageNavigator(NavigationManager navigationManager) : IPageNavigator
{
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

    public string GetPageUrlWithReturnUrl(CineastePage page, string? returnUrl) =>
        this.WithReturnUrlIfNeeded(this.GetPageUrl(page), returnUrl);

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

    public void GoToPage(CineastePage page, bool forceReload = false) =>
        this.GoTo(this.GetPageUrl(page), forceReload);

    public void GoToPage(CineastePage page, string? returnUrl, bool forceReload = false) =>
        this.GoTo(this.GetPageUrlWithReturnUrl(page, returnUrl), forceReload);

    public void GoToPageWithCurrentReturnUrl(CineastePage page, bool forceReload = false) =>
        this.GoToPage(page, this.GetReturnUrl(), forceReload);

    private void GoTo(string url, bool forceReload) =>
        navigationManager.NavigateTo(url, forceReload);

    private string WithReturnUrlIfNeeded(string url, string? returnUrl) =>
        String.IsNullOrEmpty(returnUrl) || returnUrl == "/" ? url : this.WithReturnUrl(url, returnUrl);

    private string WithReturnUrl(string url, string returnUrl) =>
        navigationManager.GetUriWithQueryParameters(
            url, new Dictionary<string, object?> { [ReturnUrlParameter] = returnUrl });

    private string GetReturnUrl()
    {
        var currentUri = new Uri(navigationManager.Uri, UriKind.Absolute);

        var queryParameters = HttpUtility.ParseQueryString(currentUri.Query);

        string? currentReturnUrl = queryParameters[ReturnUrlParameter];

        return String.IsNullOrEmpty(currentReturnUrl)
            ? new Uri(navigationManager.Uri, UriKind.Absolute)
                .MakeRelativeUri(new Uri(navigationManager.BaseUri, UriKind.Absolute))
                .ToString()
            : currentReturnUrl;
    }
}
