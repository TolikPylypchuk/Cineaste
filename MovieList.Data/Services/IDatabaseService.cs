using System.Threading.Tasks;

namespace MovieList.Data.Services
{
    public interface IDatabaseService
    {
        Task CreateDatabaseAsync(string listName);
        Task<bool> ValidateDatabaseAsync();
    }
}
