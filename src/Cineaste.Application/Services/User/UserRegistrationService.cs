using Cineaste.Application.Services.List;

using Microsoft.AspNetCore.Identity;

namespace Cineaste.Application.Services.User;

public sealed class UserRegistrationService(
    UserManager<CineasteUser> userManager,
    IUserStore<CineasteUser> userStore,
    CineasteDbContext dbContext,
    IDefaultListCreator defaultListCreator,
    TimeProvider timeProvider,
    ILogger<UserRegistrationService> logger)
    : IUserRegistrationService
{
    private readonly UserManager<CineasteUser> userManager = userManager;
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IDefaultListCreator defaultListCreator = defaultListCreator;
    private readonly TimeProvider timeProvider = timeProvider;
    private readonly ILogger<UserRegistrationService> logger = logger;

    private readonly IUserEmailStore<CineasteUser> userStore = userStore is IUserEmailStore<CineasteUser> userEmailStore
        ? userEmailStore
        : throw new ArgumentException("User store must implement IUserEmailStore", nameof(userStore));

    public async Task<(CineasteUser User, IdentityResult Result)> RegisterUser(
        string email, string password, Guid invitationCode)
    {
        this.logger.LogInformation("Registering a new user with email {Email}", email);

        var codeId = Id.For<CineasteUserInvitationCode>(invitationCode);

        var userInvitationCode = await this.dbContext
            .UserInvitationCodes
            .Where(code => code.Id == codeId && code.User == null)
            .SingleOrDefaultAsync()
            ?? throw new ArgumentException("Invitation code is invalid", nameof(invitationCode));

        var user = new CineasteUser(email);
        await userStore.SetEmailAsync(user, email, CancellationToken.None);

        user.List = defaultListCreator.CreateDefaultList();

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            userInvitationCode.AssignToUser(user, this.timeProvider.GetUtcNow());
            await this.dbContext.SaveChangesAsync();
        }

        return (user, result);
    }
}
