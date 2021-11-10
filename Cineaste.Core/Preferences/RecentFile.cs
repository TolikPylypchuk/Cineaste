namespace Cineaste.Core.Preferences;

[ToString]
[Equals(DoNotAddEqualityOperators = true)]
public sealed class RecentFile
{
    public RecentFile(string name, string path, DateTime closed)
    {
        this.Name = name;
        this.Path = path;
        this.Closed = closed;
    }

    public string Name { get; }
    public string Path { get; }
    public DateTime Closed { get; }
}
