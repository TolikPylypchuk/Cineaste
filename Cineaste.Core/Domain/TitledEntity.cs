namespace Cineaste.Core.Domain;

public abstract class TitledEntity<TEntity> : Entity<TEntity>
    where TEntity : TitledEntity<TEntity>
{
    private readonly List<Title> titles;

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    protected TitledEntity(Id<TEntity> id)
        : base(id) =>
        this.titles = [];

    protected TitledEntity(Id<TEntity> id, IEnumerable<Title> titles)
        : base(id) =>
        this.titles = titles.ToList();

    public Title AddTitle(string name, bool isOriginal)
    {
        int priority = this.titles
            .Where(title => title.IsOriginal == isOriginal)
            .Max(title => title.Priority) + 1;

        var title = new Title(name, priority, isOriginal);

        this.titles.Add(title);

        return title;
    }

    public void RemoveTitle(string name, bool isOriginal) =>
        this.titles.RemoveAll(title => title.Name == name && title.IsOriginal == isOriginal);

    public IReadOnlyCollection<Title> ReplaceTitles(IEnumerable<string> names, bool isOriginal)
    {
        var namesList = Require.NotNull(names).ToList();

        this.ValidateTitles(namesList, nameof(names));

        this.titles.RemoveAll(title => title.IsOriginal == isOriginal);

        var newTitles = namesList
            .Select((name, index) => new Title(name, index + 1, isOriginal))
            .ToList();

        this.titles.AddRange(newTitles);

        return newTitles.AsReadOnly();
    }

    protected virtual void ValidateTitles(IReadOnlyCollection<string> names, string paramName)
    {
        if (names.Count == 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The list of title names is empty");
        }
    }
}
