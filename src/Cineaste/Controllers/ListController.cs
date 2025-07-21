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
    [EndpointSummary("Get list items")]
    [ProducesResponseType<OffsettableData<ListItemModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OffsettableData<ListItemModel>>> GetListItems(int offset, int size) =>
        this.Ok(await listService.GetListItems(offset, size));

    [HttpGet("items/{id}")]
    [EndpointSummary("Get list item by ID")]
    [ProducesResponseType<ListItemModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListItemModel>> GetListItem(Guid id) =>
        this.Ok(await listService.GetListItem(id));

    [HttpGet("items/parent-franchise-{parentFranchiseId}/{sequenceNumber}")]
    [EndpointSummary("Get list item by parent franchise ID and sequence number")]
    [ProducesResponseType<ListItemModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListItemModel>> GetListItemByParentFranchise(
        Guid parentFranchiseId,
        int sequenceNumber) =>
        this.Ok(await listService.GetListItemByParentFranchise(parentFranchiseId, sequenceNumber));
}
