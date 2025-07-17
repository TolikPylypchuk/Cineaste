namespace Cineaste.Client.Api;

public interface IListApi
{
    [Get("/list")]
    Task<IApiResponse<ListModel>> GetList();

    [Get("/list/items")]
    Task<IApiResponse<OffsettableData<ListItemModel>>> GetListItems(
        [Query] int offset, [Query] int size, CancellationToken token);
}
