namespace Cineaste.Identity;

public sealed class CineasteUser : IdentityUser<Id<CineasteUser>>
{
    public CineasteUser()
    {
        Id = Core.Domain.Id.Create<CineasteUser>();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public CineasteUser(string userName)
        : this() =>
        this.UserName = userName;
}
