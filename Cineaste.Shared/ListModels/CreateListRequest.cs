namespace Cineaste.Shared.ListModels;

public sealed record CreateListRequest(
    string Name,
    string Culture,
    string DefaultSeasonTitle,
    string DefaultSeasonOriginalTitle) : IValidatable<CreateListRequest>
{
    public static IValidator<CreateListRequest> CreateValidator() =>
        new CreateListRequestValidator();
}
