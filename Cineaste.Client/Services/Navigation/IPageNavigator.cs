namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components.Routing;

public interface IPageNavigator
{
    void GoToHomePage();

    void GoToListPage(string handle);

    void GoToCreateListPage();

    event EventHandler<LocationChangedEventArgs> PageChanged;
}
