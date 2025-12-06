namespace Cineaste.Endpoints;

public static class CultureEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapCultureEndpoints()
        {
            var cultures = endpoints.MapGroup("/cultures")
                .WithTags("Cultures");

            cultures.MapGet("/", GetAllCultures)
                .WithName(nameof(GetAllCultures))
                .WithSummary("Get all supported cultures");

            return cultures;
        }
    }

    public static Ok<List<SimpleCultureModel>> GetAllCultures(CultureProvider cultureProvider) =>
        TypedResults.Ok(cultureProvider.GetAllCultures());
}
