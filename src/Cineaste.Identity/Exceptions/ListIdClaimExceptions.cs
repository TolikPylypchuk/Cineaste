namespace Cineaste.Identity.Exceptions;

public class ListIdClaimMissingException(Exception? innerException = null)
    : Exception($"Claim '{CineasteClaims.ListId}' is missing", innerException);

public class ListIdClaimInvalidException(Exception? innerException = null)
    : Exception($"Claim '{CineasteClaims.ListId}' is invalid - must be a GUID", innerException);
