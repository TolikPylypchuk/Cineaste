namespace Cineaste.Server.Services;

public interface IListService
{
    Task<List<SimpleListModel>> GetAllLists();
    Task<ListModel> GetList(string handle);

    Task<SimpleListModel> CreateList(Validated<CreateListRequest> request);
}
