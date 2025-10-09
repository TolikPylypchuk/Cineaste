namespace Cineaste.Client.Services.Navigation;

public interface IPageNavigator
{
    string HomePage { get; }

    string LoginPage { get; }

    string RegisterPage { get; }

    string ListPage { get; }

    string ListSettingsPage { get; }

    void GoToHomePage();

    void GoToLoginPage(bool forceReload = false);

    void GoToRegsiterPage();

    void GoToListPage();

    void GoToListSettingsPage();

    void GoToPage(string? url);
}
