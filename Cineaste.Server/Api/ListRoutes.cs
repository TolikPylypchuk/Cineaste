namespace Cineaste.Server.Api;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes) =>
        routes.MapGet("/api/list", GetList);

    private static async Task<IResult> GetList(ListService listService) =>
        Results.Ok(await listService.GetList());
}
