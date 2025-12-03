using System.Security.Claims;

namespace Cineaste.Identity;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Id<CineasteList>? GetListId() =>
            principal.FindFirst(CineasteClaims.ListId) is Claim claim
                ? Id.For<CineasteList>(Guid.Parse(claim.Value))
                : null;
    }
}
