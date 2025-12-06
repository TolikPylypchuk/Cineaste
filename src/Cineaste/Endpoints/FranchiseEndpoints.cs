using Cineaste.Application.Services.Poster;

namespace Cineaste.Endpoints;

public static class FranchiseEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapFranchiseEndpoints()
        {
            var franchises = endpoints.MapGroup("/franchises")
                .RequireAuthorization()
                .WithTags("Franchises");

            franchises.MapGet("/{id}", GetFranchise)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(GetFranchise))
                .WithSummary("Get a franchise");

            franchises.MapPost("/", AddFranchise)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithName(nameof(AddFranchise))
                .WithSummary("Add a franchise to the list");

            franchises.MapPut("/{id}", UpdateFranchise)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(UpdateFranchise))
                .WithSummary("Update a franchise");

            franchises.MapDelete("/{id}", RemoveFranchise)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(RemoveFranchise))
                .WithSummary("Remove a franchise from the list");

            franchises.MapGet("/{id}/poster", GetFranchisePoster)
                .ProducesPosterContentTypes()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(GetFranchisePoster))
                .WithSummary("Get a poster for a movie");

            franchises.MapPut("/{id}/poster", SetFranchisePoster)
                .AcceptsPosterContentTypes()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
                .WithName(nameof(SetFranchisePoster))
                .WithSummary("Set a poster for a franchise");

            franchises.MapPut("/{id}/poster", SetIndirectFranchisePoster)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
                .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
                .WithName(nameof(SetIndirectFranchisePoster))
                .WithSummary("Set a poster for a franchise");

            franchises.MapDelete("/{id}/poster", RemoveFranchisePoster)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithName(nameof(RemoveFranchisePoster))
                .WithSummary("Remove a poster for a franchise");

            return franchises;
        }
    }

    private static async Task<Ok<FranchiseModel>> GetFranchise(
        Guid id,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await franchiseService.GetFranchise(principal.ListId, Id.For<Franchise>(id), token));

    private static async Task<Created<FranchiseModel>> AddFranchise(
        FranchiseRequest request,
        FranchiseService franchiseService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken token)
    {
        var franchise = await franchiseService.AddFranchise(httpContext.User.ListId, request.Validated(), token);
        var uri = linkGenerator.GetUriByName(httpContext, nameof(GetFranchise), new { id = franchise.Id });
        return TypedResults.Created(uri, franchise);
    }

    private static async Task<Ok<FranchiseModel>> UpdateFranchise(
        Guid id,
        FranchiseRequest request,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await franchiseService.UpdateFranchise(
            principal.ListId, Id.For<Franchise>(id), request.Validated(), token));

    private static async Task<NoContent> RemoveFranchise(
        Guid id,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await franchiseService.RemoveFranchise(principal.ListId, Id.For<Franchise>(id), token);
        return TypedResults.NoContent();
    }

    private static async Task<FileContentHttpResult> GetFranchisePoster(
        Guid id,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await franchiseService.GetFranchisePoster(principal.ListId, Id.For<Franchise>(id), token);
        return TypedResults.File(poster.Data, poster.Type);
    }

    private static async Task<Created> SetFranchisePoster(
        Guid id,
        IFormFile file,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var franchiseId = Id.For<Franchise>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await franchiseService.SetFranchisePoster(principal.ListId, franchiseId, content, token);

        return TypedResults.Created(Urls.FranchisePoster(franchiseId, posterHash));
    }

    private static async Task<Created> SetIndirectFranchisePoster(
        Guid id,
        PosterRequestBase request,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var franchiseId = Id.For<Franchise>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await franchiseService.SetFranchisePoster(principal.ListId, franchiseId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await franchiseService.SetFranchisePoster(
                    principal.ListId, franchiseId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(Urls.FranchisePoster(franchiseId, posterHash));
    }

    private static async Task<NoContent> RemoveFranchisePoster(
        Guid id,
        FranchiseService franchiseService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await franchiseService.RemoveFranchisePoster(principal.ListId, Id.For<Franchise>(id), token);
        return TypedResults.NoContent();
    }
}
