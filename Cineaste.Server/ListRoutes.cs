namespace Cineaste.Server;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/lists", GetAllLists);
        routes.MapGet("/lists/{handle}", GetList);

        routes.MapGet("/cultures", GetAllCultures);

        routes.MapPost("/lists", CreateList);
    }

    private static async Task<IResult> GetAllLists(IListService listService) =>
        Results.Ok(await listService.GetAllLists());

    private static async Task<IResult> GetList(string handle, IListService listService)
    {
        var list = await listService.GetList(handle);
        return list != null ? Results.Ok(list) : Results.NotFound();
    }

    private static IResult GetAllCultures(IListService listService) =>
        Results.Ok(listService.GetAllCultures());

    private static async Task<IResult> CreateList(CreateListRequest request, IListService listService) =>
        Results.Ok(await listService.CreateList(request));
}
