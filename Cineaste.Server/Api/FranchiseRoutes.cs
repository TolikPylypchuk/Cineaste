namespace Cineaste.Server.Api;

public static class FranchiseRoutes
{
    public static void MapFranchiseRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/franchises/{id}", GetFranchise);
        routes.MapPost("/api/franchises", CreateFranchise);
    }

    private static async Task<IResult> GetFranchise(Guid id, IFranchiseService franchiseService) =>
        Results.Ok(await franchiseService.GetFranchise(Id.Create<Franchise>(id)));

    private static async Task<IResult> CreateFranchise(
        Validated<FranchiseRequest> request,
        IFranchiseService franchiseService)
    {
        var franchise = await franchiseService.CreateFranchise(request);
        return Results.Created($"/api/franchises/{franchise.Id}", franchise);
    }
}
