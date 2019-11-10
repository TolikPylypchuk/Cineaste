using System.Threading.Tasks;

namespace MovieList.Data.Services
{
    public interface ISettingsService
    {
        Task<Settings> GetSettingsAsync();
        Task UpdateSettingsAsync(Settings settings);
    }
}
