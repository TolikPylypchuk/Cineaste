namespace Cineaste.Client.Services.Navigation;

public interface IPageNavigator
{
    string GetPageUrl(CineastePage page);

    void GoToPage(CineastePage page, bool forceReload = false);

    void GoToPage(string? url, bool forceReload = false);
}
