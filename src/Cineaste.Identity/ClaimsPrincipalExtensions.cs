using System.Security.Claims;

namespace Cineaste.Identity;

public static class ClaimsPrincipalExtensions
{
    public static Id<CineasteList>? GetListId(this ClaimsPrincipal principal) =>
        principal.FindFirst(CineasteClaims.ListId) is Claim claim
            ? Id.For<CineasteList>(Guid.Parse(claim.Value))
            : null;
}
