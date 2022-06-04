namespace Cineaste.Client.Api;

public interface IListApi
{
    [Get("/lists")]
    Task<IApiResponse<List<SimpleListModel>>> GetLists();

    [Get("/lists/{handle}")]
    Task<IApiResponse<ListModel>> GetList(string handle);

    [Post("/lists")]
    Task<IApiResponse<SimpleListModel>> CreateList([Body] CreateListRequest request);
}
