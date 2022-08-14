namespace Cineaste.Server.Api;

public static class ListRoutes
{
    public static void MapListRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/lists", GetAllLists);
        routes.MapGet("/api/lists/{handle}", GetList);

        routes.MapPost("/api/lists", CreateList);
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
