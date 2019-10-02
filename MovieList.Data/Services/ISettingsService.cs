using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieList.Data.Services
{
    public interface ISettingsService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task UpdateSettingsAsync(IDictionary<string, string> settings);
    }
}
