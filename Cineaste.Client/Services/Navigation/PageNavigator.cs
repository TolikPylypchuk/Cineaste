namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public sealed class PageNavigator : IPageNavigator
{
    private readonly NavigationManager navigationManager;

    public PageNavigator(NavigationManager navigationManager) =>
        this.navigationManager = navigationManager;

    public void GoToHomePage() =>
        this.navigationManager.NavigateTo("/");

    public void GoToCreateListPage() =>
        this.navigationManager.NavigateTo("/new");

    public void GoToListPage(string handle) =>
        this.navigationManager.NavigateTo($"/list/{handle}");

    public void GoToListSettingsPage(string handle) =>
        this.navigationManager.NavigateTo($"/list/{handle}/settings");

    public event EventHandler<LocationChangedEventArgs> PageChanged
    {
        add => this.navigationManager.LocationChanged += value;
        remove => this.navigationManager.LocationChanged -= value;
    }
}
