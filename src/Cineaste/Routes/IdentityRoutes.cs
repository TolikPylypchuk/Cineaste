using Cineaste.Identity;

using Microsoft.AspNetCore.Identity;

namespace Cineaste.Routes;

public static class IdentityRoutes
{
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identityGroup = endpoints.MapGroup("/identity");

        identityGroup.MapPost("/logout", async ([FromServices] SignInManager<CineasteUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect("~/");
        }).DisableAntiforgery();

        return identityGroup;
    }
}
