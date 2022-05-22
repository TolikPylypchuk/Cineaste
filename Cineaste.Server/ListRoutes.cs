namespace Cineaste.Server;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/lists", GetAllLists);
    }

    private static async Task<List<SimpleListModel>> GetAllLists(IListService listService) =>
        await listService.GetAllLists();
}
