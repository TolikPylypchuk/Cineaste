namespace Cineaste.Client.Api;

public interface ICultureApi
{
    [Get("/cultures")]
    Task<IApiResponse<List<SimpleCultureModel>>> GetAllCultures();
}
