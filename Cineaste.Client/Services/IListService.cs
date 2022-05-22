namespace Cineaste.Client.Services;

public interface IListService
{
    Task<List<SimpleListModel>> GetLists();
}