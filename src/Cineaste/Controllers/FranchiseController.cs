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
    public async Task<ActionResult<FranchiseModel>> GetFranchise([FromRoute] Guid id) =>
        this.Ok(await franchiseService.GetFranchise(Id.For<Franchise>(id)));

    [HttpPost]
    [EndpointSummary("Add a franchise to the list")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FranchiseModel>> AddFranchise([FromBody] FranchiseRequest request)
    {
        var franchise = await franchiseService.CreateFranchise(request.Validated());
        return this.Created($"/api/franchises/{franchise.Id}", franchise);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update a franchise")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranchiseModel>> UpdateFranchise(
        [FromRoute] Guid id,
        [FromBody] FranchiseRequest request)
    {
        var franchise = await franchiseService.UpdateFranchise(Id.For<Franchise>(id), request.Validated());
        return this.Ok(franchise);
    }
}
