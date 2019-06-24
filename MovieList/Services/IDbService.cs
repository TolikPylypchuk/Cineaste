using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MovieList.Data.Models;
using MovieList.ViewModels;
using MovieList.ViewModels.ListItems;

namespace MovieList.Services
{
    public interface IDbService
    {
        Task<List<ListItem>> LoadListAsync();
        Task<ObservableCollection<KindViewModel>> LoadAllKindsAsync();

        Task SaveMovieAsync(Movie movie);
        Task SaveSeriesAsync(
            Series series,
            IEnumerable<Season> seasonsToDelete,
            IEnumerable<SpecialEpisode> episodesToDelete);

        Task SaveKindsAsync(IEnumerable<KindViewModel> kinds);

        Task ToggleWatchedAsync(ListItem item);
        Task ToggleReleasedAsync(MovieListItem item);

        public Task DeleteAsync(Movie movie);
        public Task DeleteAsync(Series series);

        Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : EntityBase;

        Task DeleteAsync<TEntity>(IEnumerable<TEntity> entity)
            where TEntity : EntityBase;

        Task<bool> CanDeleteKindAsync(KindViewModel kind);
    }
}
