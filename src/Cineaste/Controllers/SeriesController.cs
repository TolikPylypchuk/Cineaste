using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Controllers;

[ApiController]
[Route("/api/series")]
[Tags(["Series"])]
public sealed class SeriesController(SeriesService seriesService)
    : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a series")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeriesModel>> GetSeries(Guid id, CancellationToken token) =>
        this.Ok(await seriesService.GetSeries(Id.For<Series>(id), token));

    [HttpPost]
    [EndpointSummary("Add a series to the list")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SeriesModel>> AddSeries([FromBody] SeriesRequest request, CancellationToken token)
    {
        var series = await seriesService.AddSeries(request.Validated(), token);
        return this.Created($"/api/series/{series.Id}", series);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a series")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeriesModel>> UpdateSeries(
        Guid id,
        [FromBody] SeriesRequest request,
        CancellationToken token) =>
        this.Ok(await seriesService.UpdateSeries(Id.For<Series>(id), request.Validated(), token));

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a series from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveSeries(Guid id, CancellationToken token)
    {
        await seriesService.RemoveSeries(Id.For<Series>(id), token);
        return this.NoContent();
    }

    [HttpGet("{id}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a series")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSeriesPoster(Guid id, CancellationToken token)
    {
        var poster = await seriesService.GetSeriesPoster(Id.For<Series>(id), token);
        return this.File(poster.Data, poster.Type);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a series")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSeriesPoster(Guid id, IFormFile file, CancellationToken token)
    {
        var request = new BinaryContentRequest(file.OpenReadStream, file.Length, file.ContentType);
        await seriesService.SetSeriesPoster(Id.For<Series>(id), request, token);

        return this.NoContent();
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a series")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSeriesPoster(
        Guid id,
        [FromBody] PosterUrlRequest request,
        CancellationToken token)
    {
        await seriesService.SetSeriesPoster(Id.For<Series>(id), request.Validated(), token);
        return this.NoContent();
    }

    [HttpGet("{seriesId}/seasons/periods/{periodId}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a season")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSeasonPoster(Guid seriesId, Guid periodId, CancellationToken token)
    {
        var poster = await seriesService.GetSeasonPoster(Id.For<Series>(seriesId), Id.For<Period>(periodId), token);
        return this.File(poster.Data, poster.Type);
    }

    [HttpPut("{seriesId}/seasons/periods/{periodId}/poster")]
    [EndpointSummary("Set a poster for a season")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSeasonPoster(
        Guid seriesId,
        Guid periodId,
        IFormFile file,
        CancellationToken token)
    {
        var request = new BinaryContentRequest(file.OpenReadStream, file.Length, file.ContentType);
        await seriesService.SetSeasonPoster(Id.For<Series>(seriesId), Id.For<Period>(periodId), request, token);

        return this.NoContent();
    }

    [HttpPut("{seriesId}/seasons/periods/{periodId}/poster")]
    [EndpointSummary("Set a poster for a season")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSeasonPoster(
        Guid seriesId,
        Guid periodId,
        [FromBody] PosterUrlRequest request,
        CancellationToken token)
    {
        await seriesService.SetSeasonPoster(
            Id.For<Series>(seriesId), Id.For<Period>(periodId), request.Validated(), token);

        return this.NoContent();
    }

    [HttpGet("{seriesId}/special-episodes/{episodeId}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a special episode")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSpecialEpisodePoster(Guid seriesId, Guid episodeId, CancellationToken token)
    {
        var poster = await seriesService.GetSpecialEpisodePoster(
            Id.For<Series>(seriesId), Id.For<SpecialEpisode>(episodeId), token);

        return this.File(poster.Data, poster.Type);
    }

    [HttpPut("{seriesId}/special-episodes/{episodeId}/poster")]
    [EndpointSummary("Set a poster for a special episode")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        IFormFile file,
        CancellationToken token)
    {
        var request = new BinaryContentRequest(file.OpenReadStream, file.Length, file.ContentType);
        await seriesService.SetSpecialEpisodePoster(
            Id.For<Series>(seriesId), Id.For<SpecialEpisode>(episodeId), request, token);

        return this.NoContent();
    }

    [HttpPut("{seriesId}/special-episodes/{episodeId}/poster")]
    [EndpointSummary("Set a poster for a special episode")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetSpecialEpisodePoster(
        Guid seriesId,
        Guid episodeId,
        [FromBody] PosterUrlRequest request,
        CancellationToken token)
    {
        await seriesService.SetSpecialEpisodePoster(
            Id.For<Series>(seriesId), Id.For<SpecialEpisode>(episodeId), request.Validated(), token);

        return this.NoContent();
    }
}
