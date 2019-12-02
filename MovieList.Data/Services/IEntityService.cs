using System.Threading.Tasks;

namespace MovieList.Data.Services
{
    public interface IEntityService<in TEntity>
    {
        Task SaveAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
