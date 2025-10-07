using Microsoft.AspNetCore.Authorization;

namespace Cineaste.Controllers;

[ApiController]
[Route("/api/list")]
[Authorize]
[Tags(["List"])]
public sealed class ListController(ListService listService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the list of movies, series, and franchises")]
    [ProducesResponseType<ListModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListModel>> GetList(CancellationToken token) =>
        this.Ok(await listService.GetList(token));

    [HttpGet("items")]
    [EndpointSummary("Get list items")]
    [ProducesResponseType<OffsettableData<ListItemModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OffsettableData<ListItemModel>>> GetListItems(
        int offset,
        int size,
        CancellationToken token) =>
        this.Ok(await listService.GetListItems(offset, size, token));

    [HttpGet("items/standalone")]
    [EndpointSummary("Get standalone list items")]
    [ProducesResponseType<List<ListItemModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ListItemModel>>> GetStandaloneListItems(CancellationToken token) =>
        this.Ok(await listService.GetStandaloneListItems(token));

    [HttpGet("items/{id}")]
    [EndpointSummary("Get list item by ID")]
    [ProducesResponseType<ListItemModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListItemModel>> GetListItem(Guid id, CancellationToken token) =>
        this.Ok(await listService.GetListItem(id, token));

    [HttpGet("items/parent-franchise-{parentFranchiseId}/{sequenceNumber}")]
    [EndpointSummary("Get list item by parent franchise ID and sequence number")]
    [ProducesResponseType<ListItemModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListItemModel>> GetListItemByParentFranchise(
        Guid parentFranchiseId,
        int sequenceNumber,
        CancellationToken token) =>
        this.Ok(await listService.GetListItemByParentFranchise(parentFranchiseId, sequenceNumber, token));
}
