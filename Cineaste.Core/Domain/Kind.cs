namespace Cineaste.Core.Domain;

public abstract class Kind<TKind> : Entity<TKind>
    where TKind : Kind<TKind>
{
    private string name;
    private Color watchedColor;
    private Color notWatchedColor;
    private Color notReleasedColor;

    public string Name
    {
        get => this.name;

        [MemberNotNull(nameof(name))]
        set => this.name = Require.NotBlank(value);
    }

    public Color WatchedColor
    {
        get => this.watchedColor;

        [MemberNotNull(nameof(watchedColor))]
        set => this.watchedColor = Require.NotNull(value);
    }

    public Color NotWatchedColor
    {
        get => this.notWatchedColor;

        [MemberNotNull(nameof(notWatchedColor))]
        set => this.notWatchedColor = Require.NotNull(value);
    }

    public Color NotReleasedColor
    {
        get => this.notReleasedColor;

        [MemberNotNull(nameof(notReleasedColor))]
        set => this.notReleasedColor = Require.NotNull(value);
    }

    public Kind(Id<TKind> id, string name, Color watchedColor, Color notWatchedColor, Color notReleasedColor)
        : base(id)
    {
        this.Name = name;
        this.WatchedColor = watchedColor;
        this.NotWatchedColor = notWatchedColor;
        this.NotReleasedColor = notReleasedColor;
    }
}

public sealed class MovieKind : Kind<MovieKind>
{
    public MovieKind(Id<MovieKind> id, string name, Color watchedColor, Color notWatchedColor, Color notReleasedColor)
        : base(id, name, watchedColor, notWatchedColor, notReleasedColor)
    { }
}

public sealed class SeriesKind : Kind<SeriesKind>
{
    public SeriesKind(Id<SeriesKind> id, string name, Color watchedColor, Color notWatchedColor, Color notReleasedColor)
        : base(id, name, watchedColor, notWatchedColor, notReleasedColor)
    { }
}
