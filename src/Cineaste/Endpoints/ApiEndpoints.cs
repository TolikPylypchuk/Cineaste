namespace Cineaste.Endpoints;

public static class ApiEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapApiEndpoints()
        {
            var api = endpoints.MapGroup("/api");

            api.MapCultureEndpoints();
            api.MapListEndpoints();
            api.MapMovieEndpoints();
            api.MapSeriesEndpoints();
            api.MapFranchiseEndpoints();
            api.MapPosterEndpoints();

            return api;
        }
    }
}
