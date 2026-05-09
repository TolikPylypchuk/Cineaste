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
    public string Name
    {
        get;
        set => field = Require.NotBlank(value);
    }

    public Color WatchedColor { get; set; }

    public Color NotWatchedColor { get; set; }

    public Color NotReleasedColor { get; set; }

    public int SequenceNumber
    {
        get;
        set => field = Require.Positive(value);
    } = FirstSequenceNumber;

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
    : Kind<MovieKind>(id, name, watchedColor, notWatchedColor, notReleasedColor);

public sealed class SeriesKind(
    Id<SeriesKind> id,
    string name,
    Color watchedColor,
    Color notWatchedColor,
    Color notReleasedColor)
    : Kind<SeriesKind>(id, name, watchedColor, notWatchedColor, notReleasedColor);
