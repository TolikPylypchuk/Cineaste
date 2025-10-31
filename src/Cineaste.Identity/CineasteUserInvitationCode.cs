namespace Cineaste.Identity;

public sealed class CineasteUserInvitationCode(Id<CineasteUserInvitationCode> id)
    : Entity<CineasteUserInvitationCode>(id)
{
    public CineasteUser? User { get; private set; }

    public DateTimeOffset? AssignedAt { get; private set; }

    public void AssignToUser(CineasteUser user, DateTimeOffset? assignedAt)
    {
        this.User = user;
        this.AssignedAt = assignedAt;
    }
}
