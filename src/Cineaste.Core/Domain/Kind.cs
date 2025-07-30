namespace Cineaste.Core.Domain;

public interface IKind
{
    public string Name { get; set; }

    public Color WatchedColor { get; set; }

    public Color NotWatchedColor { get; set; }

    public Color NotReleasedColor { get; set; }

    public int SequenceNumber { get; set; }
}

public abstract class Kind<TKind> : Entity<TKind>, IKind
    where TKind : Kind<TKind>
{
    private string name;
    private Color watchedColor;
    private Color notWatchedColor;
    private Color notReleasedColor;
    private int sequenceNumber = FirstSequenceNumber;

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

    public int SequenceNumber
    {
        get => this.sequenceNumber;
        set => this.sequenceNumber = Require.Positive(value);
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

public sealed class MovieKind(
    Id<MovieKind> id,
    string name,
    Color watchedColor,
    Color notWatchedColor,
    Color notReleasedColor)
    : Kind<MovieKind>(id, name, watchedColor, notWatchedColor, notReleasedColor)
{
}

public sealed class SeriesKind(
    Id<SeriesKind> id,
    string name,
    Color watchedColor,
    Color notWatchedColor,
    Color notReleasedColor)
    : Kind<SeriesKind>(id, name, watchedColor, notWatchedColor, notReleasedColor)
{
}
