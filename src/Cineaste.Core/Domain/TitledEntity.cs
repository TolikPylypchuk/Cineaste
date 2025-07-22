using System.Collections.Immutable;

namespace Cineaste.Core.Domain;

public abstract class TitledEntity<TEntity> : Entity<TEntity>
    where TEntity : TitledEntity<TEntity>
{
    private readonly List<Title> allTitles;

    public IReadOnlyCollection<Title> AllTitles =>
        this.allTitles.AsReadOnly();

    public IReadOnlyCollection<Title> Titles =>
        [.. this.allTitles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.SequenceNumber)];

    public IReadOnlyCollection<Title> OriginalTitles =>
        [.. this.allTitles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.SequenceNumber)];

    public Title Title =>
        this.AllTitles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.SequenceNumber)
            .First();

    public Title OriginalTitle =>
        this.AllTitles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.SequenceNumber)
            .First();

    protected TitledEntity(Id<TEntity> id)
        : base(id) =>
        this.allTitles = [];

    protected TitledEntity(Id<TEntity> id, IEnumerable<Title> titles)
        : base(id) =>
        this.allTitles = Require.NotEmpty<List<Title>, Title>([.. titles]);

    public Title AddTitle(string name, bool isOriginal)
    {
        int sequenceNumber = isOriginal ? this.OriginalTitles.Count : this.Titles.Count;

        var title = new Title(name, sequenceNumber, isOriginal);
        this.allTitles.Add(title);

        return title;
    }

    public void RemoveTitle(string name, bool isOriginal)
    {
        this.allTitles.RemoveAll(title => title.Name == name && title.IsOriginal == isOriginal);

        int sequenceNumber = 1;
        foreach (var title in this.allTitles.Where(title => title.IsOriginal == isOriginal))
        {
            title.SequenceNumber = sequenceNumber++;
        }
    }

    public IReadOnlyCollection<Title> ReplaceTitles(IEnumerable<string> names, bool isOriginal)
    {
        var namesList = Require.NotNull(names).ToImmutableList();

        this.ValidateTitles(namesList, nameof(names));

        this.allTitles.RemoveAll(title => title.IsOriginal == isOriginal);

        var newTitles = namesList
            .Select((name, index) => new Title(name, index + 1, isOriginal))
            .ToImmutableList();

        this.allTitles.AddRange(newTitles);

        return newTitles;
    }

    protected virtual void ValidateTitles(IReadOnlyCollection<string> names, string paramName)
    {
        if (names.Count == 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The list of title names is empty");
        }
    }
}
