namespace Cineaste.Server.Services;

public interface IListService
{
    Task<List<SimpleListModel>> GetAllLists();
}
