namespace Cineaste.Server.Api;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/lists", GetAllLists);
        routes.MapGet("/lists/{handle}", GetList);

        routes.MapPost("/lists", CreateList);
    }

    private static async Task<List<SimpleListModel>> GetAllLists(IListService listService) =>
        await listService.GetAllLists();

    private static async Task<ListModel> GetList(string handle, IListService listService) =>
        await listService.GetList(handle);

    private static async Task<SimpleListModel> CreateList(
        Validated<CreateListRequest> request,
        IListService listService) =>
        await listService.CreateList(request);
}
