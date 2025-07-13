namespace Cineaste.Controllers;

[ApiController]
[Route("/api/cultures")]
[Tags(["Cultures"])]
public sealed class CultureController(CultureProvider cultureProvider) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all cultures supported by the application")]
    [ProducesResponseType<List<SimpleCultureModel>>(StatusCodes.Status200OK)]
    public ActionResult<List<SimpleCultureModel>> GetAllCultures() =>
        this.Ok(cultureProvider.GetAllCultures());
}
