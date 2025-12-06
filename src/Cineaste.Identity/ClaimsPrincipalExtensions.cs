using System.Security.Claims;

using Cineaste.Identity.Exceptions;

namespace Cineaste.Identity;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Id<CineasteList> ListId =>
            principal.FindFirst(CineasteClaims.ListId) is Claim claim
                ? Id.For<CineasteList>(ParseListId(claim.Value))
                : throw new ListIdClaimMissingException();
    }

    private static Guid ParseListId(string value) =>
        Guid.TryParse(value, out var listId)
            ? listId
            : throw new ListIdClaimInvalidException();
}
