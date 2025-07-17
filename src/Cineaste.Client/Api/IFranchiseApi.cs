namespace Cineaste.Client.Api;

public interface IFranchiseApi
{
    [Get("/franchises/{id}")]
    public Task<IApiResponse<FranchiseModel>> GetFranchise(Guid id);

    [Post("/franchises")]
    public Task<IApiResponse<FranchiseModel>> AddFranchise([Body] FranchiseRequest request);

    [Put("/franchises/{id}")]
    public Task<IApiResponse<FranchiseModel>> UpdateFranchise(Guid id, [Body] FranchiseRequest request);

    [Delete("/franchises/{id}")]
    public Task<IApiResponse> RemoveFranchise(Guid id);
}
