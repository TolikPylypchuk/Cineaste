using System.Collections.Generic;
using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IDatabaseService
    {
        Task CreateDatabaseAsync(Settings settings, IEnumerable<Kind> initialKinds);
        Task<bool> ValidateDatabaseAsync();
    }
}
