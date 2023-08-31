namespace Cineaste.Client.Services.Navigation;

public interface IPageNavigator
{
    string HomePage();

    void GoToHomePage();

    string ListPage();

    void GoToListPage();

    string ListSettingsPage();

    void GoToListSettingsPage();
}
