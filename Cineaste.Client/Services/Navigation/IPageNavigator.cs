namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components.Routing;

public interface IPageNavigator
{
    string HomePage();

    void GoToHomePage();

    string CreateListPage();

    void GoToCreateListPage();

    string ListPage(string handle);

    void GoToListPage(string handle);

    string ListSettingsPage(string handle);

    void GoToListSettingsPage(string handle);

    event EventHandler<LocationChangedEventArgs> PageChanged;
}
