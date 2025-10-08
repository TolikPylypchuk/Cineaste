namespace Cineaste.Client.Api;

public interface IIdentityApi
{
    [Post("/identity/login")]
    public Task<IApiResponse> Login(
        [Body] LoginRequest request,
        [Query] bool useCookies,
        [Query] bool useSessionCookies);
}
