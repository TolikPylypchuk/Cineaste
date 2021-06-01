using System.Collections.Generic;

using Cineaste.Data.Models;

namespace Cineaste.Data.Services
{
    public interface ISettingsEntityService<TEntity>
        where TEntity : EntityBase
    {
        IEnumerable<TEntity> GetAll();
        void UpdateAll(IEnumerable<TEntity> entities);
    }
}
