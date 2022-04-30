namespace Cineaste.Core.Domain;

public sealed record Title
{
    public string Name { get; }
    public int Priority { get; }
    public bool IsOriginal { get; }

    public Title(string name, int priority, bool isOriginal)
    {
        this.Name = Require.NotBlank(name);
        this.Priority = Require.Positive(priority);
        this.IsOriginal = isOriginal;
    }
}
