namespace Cineaste.Server.Api;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/lists", GetAllLists);
        routes.MapGet("/lists/{handle}", GetList);

        routes.MapPost("/lists", CreateList);
    }

    private static async Task<IResult> GetAllLists(IListService listService) =>
        Results.Ok(await listService.GetAllLists());

    private static async Task<IResult> GetList(string handle, IListService listService) =>
        Results.Ok(await listService.GetList(handle));

    private static async Task<IResult> CreateList(Validated<CreateListRequest> request, IListService listService)
    {
        var response = await listService.CreateList(request);
        return Results.Created($"/api/lists/{response.Handle}", response);
    }
}
