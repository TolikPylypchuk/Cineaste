namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components;

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
}
