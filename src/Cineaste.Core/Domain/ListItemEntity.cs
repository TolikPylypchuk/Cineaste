namespace Cineaste.Core.Domain;

public abstract class ListItemEntity<TEntity> : TitledEntity<TEntity>
    where TEntity : ListItemEntity<TEntity>
{
    public ListItem? ListItem { get; set; }

    protected ListItemEntity(Id<TEntity> id)
        : base(id)
    { }

    protected ListItemEntity(Id<TEntity> id, IEnumerable<Title> titles)
        : base(id, titles)
    { }
}
