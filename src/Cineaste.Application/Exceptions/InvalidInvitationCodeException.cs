namespace Cineaste.Application.Exceptions;

public sealed class InvalidInvitationCodeException(Guid invitationCode, Exception? innerException = null)
    : Exception($"Invitation code '{invitationCode}' is invalid", innerException)
{
    public Guid InvitationCode { get; } = invitationCode;
}
