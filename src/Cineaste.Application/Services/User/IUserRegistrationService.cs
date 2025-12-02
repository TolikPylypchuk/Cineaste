using Microsoft.AspNetCore.Identity;

namespace Cineaste.Application.Services.User;

public interface IUserRegistrationService
{
    Task<(CineasteUser User, IdentityResult Result)> RegisterUser(string email, string password, Guid invitationCode);
}
