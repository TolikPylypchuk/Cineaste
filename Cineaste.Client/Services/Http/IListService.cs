namespace Cineaste.Client.Services.Http;

public interface IListService
{
    Task<List<SimpleListModel>> GetLists();
    Task<ListModel?> GetList(string handle);
}
