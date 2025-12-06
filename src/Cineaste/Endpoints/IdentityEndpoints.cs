using Microsoft.AspNetCore.Identity;

namespace Cineaste.Endpoints;

public static class IdentityEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointConventionBuilder MapAdditionalIdentityEndpoints()
        {
            var identity = endpoints.MapGroup("/identity")
                .WithTags("Identity");

            identity.MapPost("/logout", LogOut)
                .WithName(nameof(LogOut))
                .WithSummary("Log the current user out");

            return identity;
        }
    }

    private static async Task<RedirectHttpResult> LogOut(SignInManager<CineasteUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return TypedResults.LocalRedirect("~/");
    }
}
