namespace Cineaste.Data.Services;

public interface IEntityService<in TEntity>
{
    void Save(TEntity entity);
    void Delete(TEntity entity);
}
