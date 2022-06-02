namespace Cineaste.Server.Services;

public interface IListService
{
    Task<List<SimpleListModel>> GetAllLists();
    Task<ListModel?> GetList(string handle);

    List<ListCultureModel> GetAllCultures();

    Task<SimpleListModel> CreateList(CreateListRequest request);
}
