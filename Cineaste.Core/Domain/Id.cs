namespace Cineaste.Core.Domain;

public record struct Id<T>(Guid Value) : IComparable<Id<T>>, IComparable
{
    public int CompareTo(Id<T> other) =>
        this.Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj) =>
        obj != null ? this.CompareTo((Id<T>)obj) : 1;
}

public static class Id
{
    public static Id<T> CreateNew<T>() =>
        new(Guid.NewGuid());

    public static Id<T> Create<T>(Guid id) =>
        new(id);

    public static Id<T> CreateFromNullable<T>(Guid? id) =>
        new(id is not null ? id.Value : Guid.NewGuid());
}
