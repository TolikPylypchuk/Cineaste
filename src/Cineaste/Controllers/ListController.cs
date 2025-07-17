namespace Cineaste.Controllers;

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

    [HttpGet("items")]
    [EndpointSummary("Get items from the list of movies, series, and franchises")]
    [ProducesResponseType<OffsettableData<ListItemModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OffsettableData<ListItemModel>>> GetListItems(int offset, int size) =>
        this.Ok(await listService.GetListItems(offset, size));
}
