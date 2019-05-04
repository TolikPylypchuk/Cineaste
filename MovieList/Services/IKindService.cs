using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MovieList.ViewModels;

namespace MovieList.Services
{
    public interface IKindService
    {
        Task<ObservableCollection<KindViewModel>> LoadAllKindsAsync();
        Task SaveKindsAsync(IEnumerable<KindViewModel> kinds);
        Task<bool> CanRemoveKindAsync(KindViewModel kind);
    }
}
