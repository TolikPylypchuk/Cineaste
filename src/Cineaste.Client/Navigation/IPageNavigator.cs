namespace Cineaste.Client.Navigation;

public interface IPageNavigator
{
    string GetPageUrl(CineastePage page);

    string GetPageUrlWithReturnUrl(CineastePage page, string? returnUrl);

    void GoToPage(string? url, bool forceReload = false);

    void GoToPage(CineastePage page, bool forceReload = false);

    void GoToPage(CineastePage page, string? returnUrl, bool forceReload = false);

    void GoToPageWithCurrentReturnUrl(CineastePage page, bool forceReload = false);
}
