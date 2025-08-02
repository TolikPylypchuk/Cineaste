namespace Cineaste.Client.Api;

public interface IListApi
{
    [Get("/list")]
    Task<IApiResponse<ListModel>> GetList();

    [Get("/list/items")]
    Task<IApiResponse<OffsettableData<ListItemModel>>> GetListItems(
        [Query] int offset, [Query] int size, CancellationToken token);

    [Get("/list/items/standalone")]
    Task<IApiResponse<ImmutableList<ListItemModel>>> GetStandaloneListItems();

    [Get("/list/items/{id}")]
    Task<IApiResponse<ListItemModel>> GetListItem(Guid id);

    [Get("/list/items/parent-franchise-{parentFranchiseId}/{sequenceNumber}")]
    Task<IApiResponse<ListItemModel>> GetListItemByParentFranchise(
        Guid parentFranchiseId, int sequenceNumber);
}
