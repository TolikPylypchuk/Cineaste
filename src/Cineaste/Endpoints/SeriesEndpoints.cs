using Cineaste.Application.Services.Poster;

namespace Cineaste.Endpoints;

public static class SeriesEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapSeriesEndpoints()
        {
            var series = endpoints.MapGroup("/series")
                .RequireAuthorization()
                .WithTags("Series");

            series.MapGet("/{id}", GetSeries)
                .ProducesSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetSeries))
                .WithSummary("Get a series");

            series.MapPost("/", AddSeries)
                .ProducesSeriesRequestValidationProblem()
                .ProducesSeriesKindNotFoundProblem()
                .ProducesFranchiseNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(AddSeries))
                .WithSummary("Add a series to the list");

            series.MapPut("/{id}", UpdateSeries)
                .ProducesSeriesRequestValidationProblem()
                .ProducesSeriesNotFoundProblem()
                .ProducesSeriesKindNotFoundProblem()
                .ProducesFranchiseNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(UpdateSeries))
                .WithSummary("Update a series");

            series.MapDelete("/{id}", RemoveSeries)
                .ProducesSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveSeries))
                .WithSummary("Remove a series from the list");

            series.MapGet("/{id}/poster", GetSeriesPoster)
                .ProducesPosterContentTypes()
                .ProducesSeriesNotFoundProblem()
                .ProducesSeriesPosterNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetSeriesPoster))
                .WithSummary("Get a poster for a series");

            series.MapPut("/{id}/poster", SetSeriesPoster)
                .AcceptsPosterContentTypes()
                .ProducesPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetSeriesPoster))
                .WithSummary("Set a poster for a series");

            series.MapPut("/{id}/poster", SetIndirectSeriesPoster)
                .ProducesImdbPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetIndirectSeriesPoster))
                .WithSummary("Set a poster for a series");

            series.MapDelete("/{id}/poster", RemoveSeriesPoster)
                .ProducesSeriesNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveSeriesPoster))
                .WithSummary("Remove a poster for a series");

            series.MapGet("/{seriesId}/seasons/periods/{periodId}/poster", GetSeasonPoster)
                .ProducesPosterContentTypes()
                .ProducesSeriesNotFoundProblem()
                .ProducesPeriodNotFoundProblem()
                .ProducesSeasonPosterNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetSeasonPoster))
                .WithSummary("Get a poster for a season");

            series.MapPut("/{seriesId}/seasons/periods/{periodId}/poster", SetSeasonPoster)
                .AcceptsPosterContentTypes()
                .ProducesPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesPeriodNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetSeasonPoster))
                .WithSummary("Set a poster for a season");

            series.MapPut("/{seriesId}/seasons/periods/{periodId}/poster", SetIndirectSeasonPoster)
                .ProducesImdbPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesPeriodNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetIndirectSeasonPoster))
                .WithSummary("Set a poster for a season");

            series.MapDelete("/{seriesId}/seasons/periods/{periodId}/poster", RemoveSeasonPoster)
                .ProducesSeriesNotFoundProblem()
                .ProducesPeriodNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveSeasonPoster))
                .WithSummary("Remove a poster for a season");

            series.MapGet("/{seriesId}/special-episodes/{episodeId}/poster", GetSpecialEpisodePoster)
                .ProducesPosterContentTypes()
                .ProducesSeriesNotFoundProblem()
                .ProducesSpecialEpisodeNotFoundProblem()
                .ProducesSpecialEpisodePosterNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(GetSpecialEpisodePoster))
                .WithSummary("Get a poster for a special episode");

            series.MapPut("/{seriesId}/special-episodes/{episodeId}/poster", SetSpecialEpisodePoster)
                .AcceptsPosterContentTypes()
                .ProducesPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesSpecialEpisodeNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetSpecialEpisodePoster))
                .WithSummary("Set a poster for a special episode");

            series.MapPut("/{seriesId}/special-episodes/{episodeId}/poster", SetIndirectSpecialEpisodePoster)
                .ProducesImdbPosterProblems()
                .ProducesSeriesNotFoundProblem()
                .ProducesSpecialEpisodeNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(SetIndirectSpecialEpisodePoster))
                .WithSummary("Set a poster for a special episode");

            series.MapDelete("/{seriesId}/special-episodes/{episodeId}/poster", RemoveSpecialEpisodePoster)
                .ProducesSeriesNotFoundProblem()
                .ProducesSpecialEpisodeNotFoundProblem()
                .ProducesListNotFoundProblem()
                .WithName(nameof(RemoveSpecialEpisodePoster))
                .WithSummary("Remove a poster for a special episode");

            return series;
        }
    }

    public static async Task<Ok<SeriesModel>> GetSeries(
        Guid id,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await seriesService.GetSeries(principal.ListId, Id.For<Series>(id), token));

    public static async Task<Created<SeriesModel>> AddSeries(
        SeriesRequest request,
        SeriesService seriesService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken token)
    {
        var series = await seriesService.AddSeries(httpContext.User.ListId, request.Validated(), token);
        var uri = linkGenerator.GetUriByName(httpContext, nameof(GetSeries), new { id = series.Id });
        return TypedResults.Created(uri, series);
    }

    public static async Task<Ok<SeriesModel>> UpdateSeries(
        Guid id,
        SeriesRequest request,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token) =>
        TypedResults.Ok(await seriesService.UpdateSeries(
            principal.ListId, Id.For<Series>(id), request.Validated(), token));

    public static async Task<NoContent> RemoveSeries(
        Guid id,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await seriesService.RemoveSeries(principal.ListId, Id.For<Series>(id), token);
        return TypedResults.NoContent();
    }

    public static async Task<FileContentHttpResult> GetSeriesPoster(
        Guid id,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await seriesService.GetSeriesPoster(principal.ListId, Id.For<Series>(id), token);
        return TypedResults.File(poster.Data, poster.Type);
    }

    public static async Task<Created> SetSeriesPoster(
        Guid id,
        IFormFile file,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var seriesId = Id.For<Series>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await seriesService.SetSeriesPoster(principal.ListId, seriesId, content, token);

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(seriesId, posterHash));
    }

    public static async Task<Created> SetIndirectSeriesPoster(
        Guid id,
        PosterRequestBase request,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var seriesId = Id.For<Series>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await seriesService.SetSeriesPoster(principal.ListId, seriesId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await seriesService.SetSeriesPoster(principal.ListId, seriesId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(seriesId, posterHash));
    }

    public static async Task<NoContent> RemoveSeriesPoster(
        Guid id,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await seriesService.RemoveSeriesPoster(principal.ListId, Id.For<Series>(id), token);
        return TypedResults.NoContent();
    }

    public static async Task<FileContentHttpResult> GetSeasonPoster(
        Guid seriesId,
        Guid periodId,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await seriesService.GetSeasonPoster(
            principal.ListId, Id.For<Series>(seriesId), Id.For<Period>(periodId), token);

        return TypedResults.File(poster.Data, poster.Type);
    }

    public static async Task<Created> SetSeasonPoster(
        Guid seriesId,
        Guid periodId,
        IFormFile file,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var typedSeriesId = Id.For<Series>(seriesId);
        var typedPeriodId = Id.For<Period>(periodId);

        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await seriesService.SetSeasonPoster(
            principal.ListId, typedSeriesId, typedPeriodId, content, token);

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(typedSeriesId, typedPeriodId, posterHash));
    }

    public static async Task<Created> SetIndirectSeasonPoster(
        Guid seriesId,
        Guid periodId,
        PosterRequestBase request,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var typedSeriesId = Id.For<Series>(seriesId);
        var typedPeriodId = Id.For<Period>(periodId);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await seriesService.SetSeasonPoster(
                    principal.ListId, typedSeriesId, typedPeriodId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await seriesService.SetSeasonPoster(
                    principal.ListId, typedSeriesId, typedPeriodId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(typedSeriesId, typedPeriodId, posterHash));
    }

    public static async Task<NoContent> RemoveSeasonPoster(
        Guid seriesId,
        Guid periodId,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await seriesService.RemoveSeasonPoster(
            principal.ListId, Id.For<Series>(seriesId), Id.For<Period>(periodId), token);

        return TypedResults.NoContent();
    }

    public static async Task<FileContentHttpResult> GetSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var poster = await seriesService.GetSpecialEpisodePoster(
            principal.ListId, Id.For<Series>(seriesId), Id.For<SpecialEpisode>(episodeId), token);

        return TypedResults.File(poster.Data, poster.Type);
    }

    public static async Task<Created> SetSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        IFormFile file,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var typedSeriesId = Id.For<Series>(seriesId);
        var typedEpisodeId = Id.For<SpecialEpisode>(episodeId);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            principal.ListId, typedSeriesId, typedEpisodeId, content, token);

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(typedSeriesId, typedEpisodeId, posterHash));
    }

    public static async Task<Created> SetIndirectSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        PosterRequestBase request,
        SeriesService seriesService,
        IPosterUrlProvider posterUrlProvider,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        var typedSeriesId = Id.For<Series>(seriesId);
        var typedEpisodeId = Id.For<SpecialEpisode>(episodeId);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest => await seriesService.SetSpecialEpisodePoster(
                principal.ListId, typedSeriesId, typedEpisodeId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest => await seriesService.SetSpecialEpisodePoster(
                principal.ListId, typedSeriesId, typedEpisodeId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return TypedResults.Created(posterUrlProvider.GetPosterUrl(typedSeriesId, typedEpisodeId, posterHash));
    }

    public static async Task<NoContent> RemoveSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        SeriesService seriesService,
        ClaimsPrincipal principal,
        CancellationToken token)
    {
        await seriesService.RemoveSpecialEpisodePoster(
            principal.ListId, Id.For<Series>(seriesId), Id.For<SpecialEpisode>(episodeId), token);

        return TypedResults.NoContent();
    }
}
