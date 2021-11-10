namespace Cineaste.Data;

public class IdEqualityComparer<TEntity> : EqualityComparer<TEntity>
    where TEntity : EntityBase
{
    private IdEqualityComparer()
    { }

    public static IdEqualityComparer<TEntity> Instance { get; } = new IdEqualityComparer<TEntity>();

    public override bool Equals([AllowNull] TEntity x, [AllowNull] TEntity y) =>
        (x, y) switch
        {
            (null, null) => true,
            (_, null) => false,
            (null, _) => false,
            var (left, right) => left.Id == right.Id
        };

    public override int GetHashCode([AllowNull] TEntity obj) =>
        obj?.Id.GetHashCode() ?? 0;
}
