namespace Cineaste.Controllers;

[ApiController]
[Route("/api/series")]
[Tags(["Series"])]
public sealed class SeriesController(SeriesService seriesService) : ControllerBase
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
}
