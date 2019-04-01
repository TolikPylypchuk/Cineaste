using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Services;
using MovieList.ViewModels.ListItems;

namespace MovieList.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly App app;
        private ObservableCollection<ListItem> items = new ObservableCollection<ListItem>();

        public MovieListViewModel(App app)
            => this.app = app;

        public ObservableCollection<ListItem> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        public async Task LoadItemsAsync()
        {
            using var scope = app.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMovieListService>();

            this.Items = await service.LoadAllItemsAsync();
        }
    }
}
