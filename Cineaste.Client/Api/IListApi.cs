namespace Cineaste.Client.Api;

public interface IListApi
{
    [Get("/list")]
    Task<IApiResponse<ListModel>> GetList();
}
