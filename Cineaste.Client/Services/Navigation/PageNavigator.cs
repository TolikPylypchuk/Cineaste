namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public sealed class PageNavigator : IPageNavigator
{
    private readonly NavigationManager navigationManager;

    public PageNavigator(NavigationManager navigationManager) =>
        this.navigationManager = navigationManager;

    public string HomePage() =>
        "/";

    public void GoToHomePage() =>
        this.GoTo(this.HomePage());

    public string CreateListPage() =>
        "/new";

    public void GoToCreateListPage() =>
        this.GoTo(this.CreateListPage());

    public string ListPage(string handle) =>
        $"/list/{handle}";

    public void GoToListPage(string handle) =>
        this.GoTo(this.ListPage(handle));

    public string ListSettingsPage(string handle) =>
        $"/list/{handle}/settings";

    public void GoToListSettingsPage(string handle) =>
        this.GoTo(this.ListSettingsPage(handle));

    private void GoTo(string page) =>
        this.navigationManager.NavigateTo(page);

    public event EventHandler<LocationChangedEventArgs> PageChanged
    {
        add => this.navigationManager.LocationChanged += value;
        remove => this.navigationManager.LocationChanged -= value;
    }
}
