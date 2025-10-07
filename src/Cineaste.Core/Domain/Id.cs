namespace Cineaste.Core.Domain;

public readonly record struct Id<T>(Guid Value) : IComparable<Id<T>>, IComparable
{
    public readonly int CompareTo(Id<T> other) =>
        this.Value.CompareTo(other.Value);

    public readonly override string ToString() =>
        this.Value.ToString();

    readonly int IComparable.CompareTo(object? obj) =>
        obj != null ? this.CompareTo((Id<T>)obj) : 1;
}

public static class Id
{
    public static Id<T> Create<T>() =>
        new(Guid.CreateVersion7());

    public static Id<T> For<T>(Guid id) =>
        new(id);

    public static Id<T> ForNullable<T>(Guid? id) =>
        new(id is not null ? id.Value : Guid.CreateVersion7());
}
