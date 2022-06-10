namespace Cineaste.Shared.ListModels;

public sealed record CreateListRequest : IValidatable<CreateListRequest>
{
    public string Name { get; }
    public string Culture { get; }
    public string DefaultSeasonTitle { get; }
    public string DefaultSeasonOriginalTitle { get; }

    public CreateListRequest(
        string name,
        string culture,
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle)
    {
        this.Name = name;
        this.Culture = culture;
        this.DefaultSeasonTitle = defaultSeasonTitle;
        this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
    }

    public static IValidator<CreateListRequest> CreateValidator() =>
        new CreateListRequestValidator();
}
