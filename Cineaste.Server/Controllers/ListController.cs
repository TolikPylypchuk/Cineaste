namespace Cineaste.Server.Controllers;

[ApiController]
[Route("/api/list")]
[Tags(["List"])]
public sealed class ListController(ListService listService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the list of movies, series, and franchises")]
    [ProducesResponseType<ListModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListModel>> GetList() =>
        this.Ok(await listService.GetList());
}
