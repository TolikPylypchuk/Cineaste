namespace Cineaste.Core.Domain;

public abstract class FranchiseItemEntity<TEntity> : ListItemEntity<TEntity>
    where TEntity : FranchiseItemEntity<TEntity>
{
    public FranchiseItem? FranchiseItem { get; set; }

    protected FranchiseItemEntity(Id<TEntity> id)
        : base(id)
    { }

    protected FranchiseItemEntity(Id<TEntity> id, IEnumerable<Title> titles)
        : base(id, titles)
    { }
}
