namespace Cineaste.Core.Domain;

public readonly record struct TagCategory
{
    public string Name { get; }

    public TagCategory(string name) =>
        this.Name = Require.NotBlank(name);
}
