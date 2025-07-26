namespace Cineaste.Controllers;

[ApiController]
[Route("/api/franchises")]
[Tags(["Franchises"])]
public sealed class FranchiseController(FranchiseService franchiseService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a franchise")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranchiseModel>> GetFranchise(Guid id, CancellationToken token) =>
        this.Ok(await franchiseService.GetFranchise(Id.For<Franchise>(id), token));

    [HttpPost]
    [EndpointSummary("Add a franchise to the list")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FranchiseModel>> AddFranchise(
        [FromBody] FranchiseRequest request,
        CancellationToken token)
    {
        var franchise = await franchiseService.AddFranchise(request.Validated(), token);
        return this.Created($"/api/franchises/{franchise.Id}", franchise);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a franchise")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranchiseModel>> UpdateFranchise(
        [FromRoute] Guid id,
        [FromBody] FranchiseRequest request,
        CancellationToken token) =>
        this.Ok(await franchiseService.UpdateFranchise(Id.For<Franchise>(id), request.Validated(), token));
}
