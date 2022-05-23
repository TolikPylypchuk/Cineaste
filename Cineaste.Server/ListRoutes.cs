namespace Cineaste.Server;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/lists", GetAllLists);
        routes.MapGet("/lists/{id:guid}", GetList);
    }

    private static async Task<IResult> GetAllLists(IListService listService) =>
        Results.Ok(await listService.GetAllLists());

    private static async Task<IResult> GetList(Guid id, IListService listService)
    {
        var list = await listService.GetList(id);
        return list != null ? Results.Ok(list) : Results.NotFound();
    }
}
