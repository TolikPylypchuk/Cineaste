namespace Cineaste.Core.Domain;

public sealed record TagCategory
{
    public string Name { get; }

    public TagCategory(string name) =>
        this.Name = Require.NotBlank(name);
}
