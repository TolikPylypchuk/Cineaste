namespace Cineaste.Client.Services.Navigation;

public interface IPageNavigator
{
    void GoToHomePage();

    void GoToListPage(string handle);

    void GoToCreateListPage();
}
