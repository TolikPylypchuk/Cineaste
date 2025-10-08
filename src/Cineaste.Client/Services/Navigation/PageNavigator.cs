namespace Cineaste.Client.Services.Navigation;

public sealed class PageNavigator(NavigationManager navigationManager) : IPageNavigator
{
    private readonly NavigationManager navigationManager = navigationManager;

    public string HomePage => "/";
    public string LoginPage => "/login";
    public string RegisterPage => "/register";
    public string ListPage => "/list";
    public string ListSettingsPage => "/list/settings";

    public void GoToHomePage() =>
        this.GoTo(this.HomePage);

    public void GoToLoginPage() =>
        this.GoTo(this.LoginPage);

    public void GoToRegsiterPage() =>
        this.GoTo(this.RegisterPage);

    public void GoToListPage() =>
        this.GoTo(this.ListPage);

    public void GoToListSettingsPage() =>
        this.GoTo(this.ListSettingsPage);

    private void GoTo(string page) =>
        this.navigationManager.NavigateTo(page);
}
