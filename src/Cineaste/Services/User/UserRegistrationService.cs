using Cineaste.Identity;
using Cineaste.Services.List;

using Microsoft.AspNetCore.Identity;

namespace Cineaste.Services.User;

public sealed class UserRegistrationService(
    UserManager<CineasteUser> userManager,
    IUserStore<CineasteUser> userStore,
    IDefaultListCreator defaultListCreator,
    ILogger<UserRegistrationService> logger)
    : IUserRegistrationService
{
    private readonly UserManager<CineasteUser> userManager = userManager;
    private readonly IDefaultListCreator defaultListCreator = defaultListCreator;
    private readonly ILogger<UserRegistrationService> logger = logger;

    private readonly IUserEmailStore<CineasteUser> userStore = userStore is IUserEmailStore<CineasteUser> userEmailStore
        ? userEmailStore
        : throw new ArgumentException("User store must implement IUserEmailStore", nameof(userStore));

    public async Task<(CineasteUser User, IdentityResult Result)> RegisterUser(string email, string password)
    {
        logger.LogInformation("Registering a new user with email {Email}", email);

        var user = new CineasteUser(email);
        await userStore.SetEmailAsync(user, email, CancellationToken.None);

        user.List = defaultListCreator.CreateDefaultList();

        var result = await userManager.CreateAsync(user, password);

        return (user, result);
    }
}
