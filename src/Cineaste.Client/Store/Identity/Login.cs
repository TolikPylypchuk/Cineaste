namespace Cineaste.Client.Store.Identity;

public record LoginAction(LoginRequest LoginRequest, bool RememberUser);

public record LoginResultAction(EmptyApiResult Result) : EmptyResultAction(Result);

public sealed class IdentityEffects(IIdentityApi api)
{
    [EffectMethod]
    public async Task HandleLoginAction(LoginAction action, IDispatcher dispatcher)
    {
        var result = await api.Login(action.LoginRequest, action.RememberUser, !action.RememberUser).ToApiResultAsync();
        dispatcher.Dispatch(new LoginResultAction(result));
    }
}
