using Microsoft.AspNetCore.Authorization;

using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Controllers;

[ApiController]
[Route("/api/franchises")]
[Authorize]
[Tags(["Franchises"])]
public sealed class FranchiseController(FranchiseService franchiseService) : ControllerBase
{
    [HttpGet("{id}")]
    [EndpointSummary("Get a franchise")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranchiseModel>> GetFranchise(Guid id, CancellationToken token) =>
        this.Ok(await franchiseService.GetFranchise(this.GetCurrentListId(), Id.For<Franchise>(id), token));

    [HttpPost]
    [EndpointSummary("Add a franchise to the list")]
    [ProducesResponseType<FranchiseModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FranchiseModel>> AddFranchise(
        [FromBody] FranchiseRequest request,
        CancellationToken token)
    {
        var franchise = await franchiseService.AddFranchise(this.GetCurrentListId(), request.Validated(), token);
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
        this.Ok(await franchiseService.UpdateFranchise(
            this.GetCurrentListId(), Id.For<Franchise>(id), request.Validated(), token));

    [HttpDelete("{id}")]
    [EndpointSummary("Remove a franchise from the list")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveFranchise(Guid id, CancellationToken token)
    {
        await franchiseService.RemoveFranchise(this.GetCurrentListId(), Id.For<Franchise>(id), token);
        return this.NoContent();
    }

    [HttpGet("{id}/poster")]
    [Produces(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp)]
    [EndpointSummary("Get a poster for a franchise")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFranchisePoster(Guid id, CancellationToken token)
    {
        var poster = await franchiseService.GetFranchisePoster(this.GetCurrentListId(), Id.For<Franchise>(id), token);
        return this.File(poster.Data, poster.Type);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a franchise")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetFranchisePoster(Guid id, IFormFile file, CancellationToken token)
    {
        var franchiseId = Id.For<Franchise>(id);
        var content = new StreamableContent(file.OpenReadStream, file.Length, file.ContentType);

        var posterHash = await franchiseService.SetFranchisePoster(
            this.GetCurrentListId(), franchiseId, content, token);

        return this.Created(Urls.FranchisePoster(franchiseId, posterHash), null);
    }

    [HttpPut("{id}/poster")]
    [EndpointSummary("Set a poster for a franchise")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetFranchisePoster(
        Guid id,
        [FromBody] PosterRequestBase request,
        CancellationToken token)
    {
        var franchiseId = Id.For<Franchise>(id);

        var posterHash = request switch
        {
            PosterUrlRequest urlRequest =>
                await franchiseService.SetFranchisePoster(
                    this.GetCurrentListId(), franchiseId, urlRequest.Validated(), token),

            PosterImdbMediaRequest imdbMediaRequest =>
                await franchiseService.SetFranchisePoster(
                    this.GetCurrentListId(), franchiseId, imdbMediaRequest.Validated(), token),

            _ => throw new IncompleteMatchException("Unknown poster request type")
        };

        return this.Created(Urls.FranchisePoster(franchiseId, posterHash), null);
    }

    [HttpDelete("{id}/poster")]
    [EndpointSummary("Remove a poster for a franchise")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveFranchisePoster(Guid id, CancellationToken token)
    {
        await franchiseService.RemoveFranchisePoster(this.GetCurrentListId(), Id.For<Franchise>(id), token);
        return this.NoContent();
    }

    private Id<CineasteList> GetCurrentListId() =>
        this.User.GetListId() ?? throw new InvalidOperationException("List ID not found in the current user's claims");
}
