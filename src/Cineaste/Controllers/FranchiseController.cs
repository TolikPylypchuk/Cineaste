using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Controllers;

[ApiController]
[Route("/api/franchises")]
[Tags(["Franchises"])]
public sealed class FranchiseController(
    FranchiseService franchiseService,
    PosterContentTypeValidator posterContentTypeValidator)
    : ControllerBase
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

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a franchise from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveFranchise(Guid id, CancellationToken token)
    {
        await franchiseService.RemoveFranchise(Id.For<Franchise>(id), token);
        return this.NoContent();
    }

    [HttpGet("{id}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a franchise")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFranchisePoster(Guid id, CancellationToken token)
    {
        var poster = await franchiseService.GetFranchisePoster(Id.For<Franchise>(id), token);
        return this.File(poster.Data, poster.ContentType);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a franchise")]
    [ProducesResponseType<MovieModel>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetFranchisePoster(Guid id, IFormFile file, CancellationToken token)
    {
        posterContentTypeValidator.ValidateContentType(file.ContentType);

        var request = new PosterRequest(file.OpenReadStream(), file.Length, file.ContentType);
        await franchiseService.SetFranchisePoster(Id.For<Franchise>(id), request, token);

        return this.NoContent();
    }
}
