using System.Threading.Tasks;

namespace MovieList.Data.Services
{
    public interface IDatabaseService
    {
        Task CreateDatabaseAsync(Settings settings);
        Task<bool> ValidateDatabaseAsync();
    }
}
