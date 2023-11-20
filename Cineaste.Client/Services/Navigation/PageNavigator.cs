namespace Cineaste.Client.Services.Navigation;

using Microsoft.AspNetCore.Components;

public sealed class PageNavigator(NavigationManager navigationManager) : IPageNavigator
{
    private readonly NavigationManager navigationManager = navigationManager;

    public string HomePage() =>
        "/";

    public void GoToHomePage() =>
        this.GoTo(this.HomePage());

    public string ListPage() =>
        "/list";

    public void GoToListPage() =>
        this.GoTo(this.ListPage());

    public string ListSettingsPage() =>
        "/list/settings";

    public void GoToListSettingsPage() =>
        this.GoTo(this.ListSettingsPage());

    private void GoTo(string page) =>
        this.navigationManager.NavigateTo(page);
}
