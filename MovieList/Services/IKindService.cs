using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Services
{
    public interface IKindService
    {
        Task<ObservableCollection<Kind>> LoadAllKindsAsync();
        Task SaveKindsAsync(IEnumerable<Kind> kinds);
    }
}
