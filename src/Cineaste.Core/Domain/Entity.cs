namespace Cineaste.Core.Domain;

public abstract class Entity<TEntity>(Id<TEntity> id) : IEquatable<TEntity>
    where TEntity : Entity<TEntity>
{
    public Id<TEntity> Id { get; } = id;

    public override bool Equals(object? obj) =>
        obj is TEntity other && this.Equals(other);

    public bool Equals(TEntity? other) =>
        other is not null && this.Id.Equals(other.Id);

    public override int GetHashCode() =>
        this.Id.GetHashCode();

    public static bool operator ==(Entity<TEntity>? left, Entity<TEntity>? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<TEntity>? left, Entity<TEntity>? right) =>
        !(left == right);
}
