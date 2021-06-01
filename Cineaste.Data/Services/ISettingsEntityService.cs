using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface ISettingsEntityService<TEntity>
        where TEntity : EntityBase
    {
        IEnumerable<TEntity> GetAll();
        void UpdateAll(IEnumerable<TEntity> entities);
    }
}
