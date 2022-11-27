namespace Cineaste.Server.Api;

public static class FranchiseRoutes
{
    public static void MapFranchiseRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/franchises/{id}", GetFranchise);
    }

    private static async Task<IResult> GetFranchise(Guid id, IFranchiseService franchiseService) =>
        Results.Ok(await franchiseService.GetFranchise(Id.Create<Franchise>(id)));
}
