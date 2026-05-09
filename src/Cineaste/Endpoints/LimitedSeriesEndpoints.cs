using Cineaste.Application.Services.Poster;

namespace Cineaste.Endpoints;

public static class LimitedSeriesEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapLimitedSeriesEndpoints()
        {
            var limitedSeries = endpoints.MapGroup("/limited-series")
                .RequireAuthorization()
                .WithTags("Limited Series");

            limitedSeries.MapGet("/{id}", GetLimitedSeries)
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetLimitedSeries))
                .WithSummary("Get a limited series");

            limitedSeries.MapPost("/", AddLimitedSeries)
                .ProducesLimitedSeriesRequestValidationProblem()
                .ProducesSeriesKindNotFoundProblem()
                .ProducesFranchiseNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(AddLimitedSeries))
                .WithSummary("Add a limited series to the list");
            
            limitedSeries.MapPut("/{id}", UpdateLimitedSeries)
                .ProducesLimitedSeriesRequestValidationProblem()
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesSeriesKindNotFoundProblem()
                .ProducesFranchiseNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(UpdateLimitedSeries))
                .WithSummary("Update a limited series");

            limitedSeries.MapDelete("/{id}", RemoveLimitedSeries)
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveLimitedSeries))
                .WithSummary("Remove a limited series from the list");

            limitedSeries.MapGet("/{id}/poster", GetLimitedSeriesPoster)
                .ProducesPosterContentTypes()
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesLimitedSeriesPosterNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetLimitedSeriesPoster))
                .WithSummary("Get a poster for a limited series");

            limitedSeries.MapPut("/{id}/poster", SetLimitedSeriesPoster)
                .AcceptsPosterContentTypes()
                .ProducesPosterProblems()
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetLimitedSeriesPoster))
                .WithSummary("Set a poster for a limited series");

            limitedSeries.MapPut("/{id}/poster", SetIndirectLimitedSeriesPoster)
                .ProducesImdbPosterProblems()
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetIndirectLimitedSeriesPoster))
                .WithSummary("Set a poster for a limited series");

            limitedSeries.MapDelete("/{id}/poster", RemoveLimitedSeriesPoster)
                .ProducesLimitedSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveLimitedSeriesPoster))
                .WithSummary("Remove a poster for a limited series");

            return limitedSeries;
        }
    }

    public static async Task<Ok<LimitedSeriesModel>> GetLimitedSeries(
        Guid id,
        LimitedSeriesService limitedSeriesService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(
            await limitedSeriesService.GetLimitedSeries(principal.ListId, Id.For<LimitedSeries>(id), token));

    public static async Task<Created<LimitedSeriesModel>> AddLimitedSeries(
        LimitedSeriesRequest request,
        LimitedSeriesService limitedSeriesService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken token)
    {
        var limivedSeries = await limitedSeriesService.AddLimitedSeries(
            httpContext.User.ListId, request.Validated(), token);

        var uri = linkGenerator.GetUriByName(httpContext, nameof(GetLimitedSeries), new { id = limivedSeries.Id });
        return TypedResults.Created(uri, limivedSeries);
    }

    public static async Task<Ok<LimitedSeriesModel>> UpdateLimitedSeries(
        Guid id,
        LimitedSeriesRequest request,
        LimitedSeriesService limitedSeriesService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await limitedSeriesService.UpdateLimitedSeries(
            principal.ListId, Id.For<LimitedSeries>(id), request.Validated(), token));

    public static async Task<NoContent> RemoveLimitedSeries(
        Guid id,
        LimitedSeriesService limitedSeriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await limitedSeriesService.RemoveLimitedSeries(principal.ListId, Id.For<LimitedSeries>(id), token);
        return TypedResults.NoContent();
    }

    public static async Task<FileContentHttpResult> GetLimitedSeriesPoster(
        Guid id,
        LimitedSeriesService limitedSeriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await limitedSeriesService.GetLimitedSeriesPoster(
            principal.ListId, Id.For<LimitedSeries>(id), token);
        return TypedResults.File(poster.Data, poster.Type);
    }

    public static async Task<Created> SetLimitedSeriesPoster(
        Guid id,
        IFormFile file,
        LimitedSeriesService limitedSeriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var limitedSeriesId = Id.For<LimitedSeries>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await limitedSeriesService.SetLimitedSeriesPoster(
            principal.ListId, limitedSeriesId, content, token);

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(limitedSeriesId, posterHash));
    }

    public static async Task<Created> SetIndirectLimitedSeriesPoster(
        Guid id,
        PosterRequestBase request,
        LimitedSeriesService limitedSeriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var limitedSeriesId = Id.For<LimitedSeries>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest => await limitedSeriesService.SetLimitedSeriesPoster(
                principal.ListId, limitedSeriesId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest => await limitedSeriesService.SetLimitedSeriesPoster(
                principal.ListId, limitedSeriesId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(limitedSeriesId, posterHash));
    }

    public static async Task<NoContent> RemoveLimitedSeriesPoster(
        Guid id,
        LimitedSeriesService limitedSeriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await limitedSeriesService.RemoveLimitedSeriesPoster(principal.ListId, Id.For<LimitedSeries>(id), token);
        return TypedResults.NoContent();
    }
}
