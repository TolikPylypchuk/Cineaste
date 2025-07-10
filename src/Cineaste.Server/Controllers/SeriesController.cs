namespace Cineaste.Server.Controllers;

[ApiController]
[Route("/api/series")]
[Tags(["Series"])]
public sealed class SeriesController(SeriesService seriesService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a series")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeriesModel>> GetSeries([FromRoute] Guid id) =>
        this.Ok(await seriesService.GetSeries(Id.For<Series>(id)));

    [HttpPost]
    [EndpointSummary("Add a series to the list")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SeriesModel>> AddSeries([FromBody] SeriesRequest request)
    {
        var series = await seriesService.CreateSeries(request.Validated());
        return this.Created($"/api/series/{series.Id}", series);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a series")]
    [ProducesResponseType<SeriesModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeriesModel>> UpdateSeries(
        [FromRoute] Guid id,
        [FromBody] SeriesRequest request)
    {
        var series = await seriesService.UpdateSeries(Id.For<Series>(id), request.Validated());
        return this.Ok(series);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a series from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveSeries([FromRoute] Guid id)
    {
        await seriesService.DeleteSeries(Id.For<Series>(id));
        return this.NoContent();
    }
}
