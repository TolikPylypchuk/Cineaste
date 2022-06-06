namespace Cineaste.Client.Api;

public interface ICultureApi : IApi
{
    [Get("/cultures")]
    Task<IApiResponse<List<SimpleCultureModel>>> GetAllCultures();
}
