namespace Cineaste.Client.Services.Http;

public interface IListService
{
    Task<List<SimpleListModel>> GetLists();
    Task<ListModel?> GetList(string handle);

    Task<List<ListCultureModel>> GetAllCultures();

    Task<SimpleListModel?> CreateList(CreateListRequest request);
}
