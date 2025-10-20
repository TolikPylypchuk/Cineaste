namespace Cineaste.Identity;

public sealed class CineasteUser : IdentityUser<Id<CineasteUser>>
{
    public CineasteUser()
    {
        this.Id = Core.Domain.Id.Create<CineasteUser>();
        this.SecurityStamp = Guid.CreateVersion7().ToString();
        this.List = null!;
    }

    public CineasteUser(string userName)
        : this() =>
        this.UserName = userName;

    public CineasteList List { get; set; }
}
