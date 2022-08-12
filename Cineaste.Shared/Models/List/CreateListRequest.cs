namespace Cineaste.Shared.Models.List;

using Cineaste.Shared.Validation.List;

public sealed record CreateListRequest(
    string Name,
    string Culture,
    string DefaultSeasonTitle,
    string DefaultSeasonOriginalTitle) : IValidatable<CreateListRequest>
{
    public static IValidator<CreateListRequest> CreateValidator() =>
        new CreateListRequestValidator();
}
