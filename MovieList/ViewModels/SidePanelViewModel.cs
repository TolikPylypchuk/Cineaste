using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MovieList.Commands;
using MovieList.Data.Models;
using MovieList.Services;
using MovieList.ViewModels.FormItems;
using MovieList.Views;

namespace MovieList.ViewModels
{
    public class SidePanelViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDbService dbService;

        private SeriesFormControl? parentFormControl;

        public SidePanelViewModel(IServiceProvider serviceProvider, IDbService dbService)
        {
            this.serviceProvider = serviceProvider;
            this.dbService = dbService;

            this.OpenMovie = new DelegateCommand<Movie>(async movie => await this.OpenMovieAsync(movie));
            this.OpenSeries = new DelegateCommand<Series>(async series => await this.OpenSeriesAsync(series));
            this.OpenMovieSeries = new DelegateCommand<MovieSeries>(
                async movieSeries => await this.OpenMovieSeriesAsync(movieSeries));
            this.OpenSeriesComponent = new DelegateCommand<SeriesComponentFormItemBase>(this.OnOpenSeriesComponent);
            this.CreateMovieSeries = new DelegateCommand<EntityBase>(
                async entity => await this.CreateMovieSeriesAsync(entity));

            this.Close = new DelegateCommand(this.OnClose);

            this.GoUpToSeries = new DelegateCommand(this.OnGoUpToSeries, this.CanGoUpToSeries);
            this.GoUpToMovieSeries = new DelegateCommand<EntityBase?>(
                async entity => await this.GoUpToMovieSeriesAsync(entity), this.CanGoUpToMovieSeries);

            this.SelectNextEntry = new DelegateCommand<EntityBase?>(
                async entity => await this.SelectNextEntryAsync(entity), this.CanSelectNextEntry);
            this.SelectPreviousEntry = new DelegateCommand<EntityBase?>(
                async entity => await this.SelectPreviousEntryAsync(entity), this.CanSelectPreviousEntry);
        }

        public DelegateCommand<Movie> OpenMovie { get; }
        public DelegateCommand<Series> OpenSeries { get; }
        public DelegateCommand<MovieSeries> OpenMovieSeries { get; }
        public DelegateCommand<SeriesComponentFormItemBase> OpenSeriesComponent { get; }
        public DelegateCommand<EntityBase> CreateMovieSeries { get; }

        public DelegateCommand Close { get; }

        public DelegateCommand GoUpToSeries { get; }
        public DelegateCommand<EntityBase?> GoUpToMovieSeries { get; }

        public DelegateCommand<EntityBase?> SelectNextEntry { get; }
        public DelegateCommand<EntityBase?> SelectPreviousEntry { get; }

        public SidePanelControl SidePanelControl { get; set; }

        public event EventHandler Closed;

        private async Task OpenMovieAsync(Movie movie)
        {
            var control = new MovieFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<MovieFormViewModel>();
            control.ViewModel.MovieFormControl = control;
            control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
            control.ViewModel.Movie = new MovieFormItem(movie, control.ViewModel.AllKinds);

            this.SidePanelControl.ContentContainer.Content = control;
        }

        private async Task OpenSeriesAsync(Series series)
        {
            var control = new SeriesFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<SeriesFormViewModel>();
            control.ViewModel.SeriesFormControl = control;
            control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
            control.ViewModel.Series = new SeriesFormItem(series, control.ViewModel.AllKinds);

            this.SidePanelControl.ContentContainer.Content = control;
        }

        private async Task OpenMovieSeriesAsync(MovieSeries movieSeries)
        {
            var control = new MovieSeriesFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<MovieSeriesFormViewModel>();
            control.ViewModel.MovieSeriesFormControl = control;
            control.ViewModel.AllKinds = await this.dbService.LoadAllKindsAsync();
            control.ViewModel.MovieSeries = new MovieSeriesFormItem(movieSeries, control.ViewModel.AllKinds);

            this.SidePanelControl.ContentContainer.Content = control;
        }

        private void OnOpenSeriesComponent(SeriesComponentFormItemBase component)
        {
            switch (component)
            {
                case SeasonFormItem season:
                    if (this.SidePanelControl.ContentContainer.Content is SeriesFormControl seriesFormControl1)
                    {
                        this.parentFormControl = seriesFormControl1;
                    }

                    var seasonFormControl = new SeasonFormControl();
                    seasonFormControl.DataContext = seasonFormControl.ViewModel =
                        this.serviceProvider.GetRequiredService<SeasonFormViewModel>();
                    seasonFormControl.ViewModel.SeasonFormControl = seasonFormControl;
                    seasonFormControl.ViewModel.Season = season;

                    this.SidePanelControl.ContentContainer.Content = seasonFormControl;
                    break;
                case SpecialEpisodeFormItem episode:
                    if (this.SidePanelControl.ContentContainer.Content is SeriesFormControl seriesFormControl2)
                    {
                        this.parentFormControl = seriesFormControl2;
                    }

                    var specialEpisodeFormControl = new SpecialEpisodeFormControl();
                    specialEpisodeFormControl.DataContext = specialEpisodeFormControl.ViewModel =
                        this.serviceProvider.GetRequiredService<SpecialEpisodeFormViewModel>();
                    specialEpisodeFormControl.ViewModel.SpecialEpisodeFormControl = specialEpisodeFormControl;
                    specialEpisodeFormControl.ViewModel.SpecialEpisode = episode;

                    this.SidePanelControl.ContentContainer.Content = specialEpisodeFormControl;
                    break;
            }
        }

        private async Task CreateMovieSeriesAsync(EntityBase entity)
        {
            var movieSeries = new MovieSeries();

            var entry = new MovieSeriesEntry
            {
                MovieSeries = movieSeries
            };

            switch (entity)
            {
                case Movie movie:
                    entry.Movie = movie;
                    movie.Entry = entry;
                    break;
                case Series series:
                    entry.Series = series;
                    series.Entry = entry;
                    break;
                default:
                    throw new NotSupportedException($"Creating a movie series for {entity} is not supported.");
            }

            movieSeries.Entries.Add(entry);

            var control = new MovieSeriesFormControl();
            control.DataContext = control.ViewModel =
                this.serviceProvider.GetRequiredService<MovieSeriesFormViewModel>();
            control.ViewModel.MovieSeriesFormControl = control;

            control.ViewModel.MovieSeries = new MovieSeriesFormItem(
                movieSeries, await this.dbService.LoadAllKindsAsync());

            var component = control.ViewModel.MovieSeries.Components[0];
            component.OrdinalNumber = 1;
            component.DisplayNumber = 1;

            this.SidePanelControl.ContentContainer.Content = control;

            control.ViewModel.MovieSeries.ForceRefreshProperty(nameof(MovieSeriesFormItem.AreChangesPresent));
        }

        private void OnClose()
        {
            var control = new AddNewControl();
            control.DataContext = control.ViewModel = this.serviceProvider.GetRequiredService<AddNewViewModel>();
            this.SidePanelControl.ContentContainer.Content = control;

            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        private void OnGoUpToSeries()
        {
            this.SidePanelControl.ContentContainer.Content = this.parentFormControl;
            this.parentFormControl = null;
        }

        private bool CanGoUpToSeries()
            => this.parentFormControl != null;

        private async Task GoUpToMovieSeriesAsync(EntityBase? entity)
        {
            switch (entity)
            {
                case MovieSeriesEntry entry:
                    await this.OpenMovieSeriesAsync(entry.MovieSeries);
                    break;
                case MovieSeries movieSeries:
                    await this.OpenMovieSeriesAsync(movieSeries.ParentSeries!);
                    break;
            }
        }

        private bool CanGoUpToMovieSeries(EntityBase? entity)
            => entity switch
            {
                MovieSeriesEntry _ => true,
                MovieSeries movieSeries => movieSeries.ParentSeries != null,
                _ => false
            };

        private async Task SelectNextEntryAsync(EntityBase? entity)
        {
            (MovieSeries? movieSeries, int ordinalNumber) = entity switch
            {
                MovieSeriesEntry entry => (entry.MovieSeries, entry.OrdinalNumber),
                MovieSeries ms => (ms.ParentSeries, ms.OrdinalNumber.Value),
                _ => default
            };

            if (movieSeries == null)
            {
                return;
            }

            var nextEntry = movieSeries.Entries
                .Where(e => e.OrdinalNumber > ordinalNumber)
                .OrderBy(e => e.OrdinalNumber)
                .FirstOrDefault();

            if (nextEntry != null && nextEntry.OrdinalNumber == ordinalNumber + 1)
            {
                await this.OpenEntryAsync(nextEntry);
            } else
            {
                var nextPart = movieSeries.Parts
                    .Where(p => p.OrdinalNumber > ordinalNumber)
                    .OrderBy(p => p.OrdinalNumber)
                    .FirstOrDefault();

                if (nextPart != null)
                {
                    await this.OpenEntryAsync(nextPart.GetFirstEntry());
                }
            }
        }

        private bool CanSelectNextEntry(EntityBase? entity)
        {
            (MovieSeries? movieSeries, int ordinalNumber) = entity switch
            {
                MovieSeriesEntry entry => (entry.MovieSeries, entry.OrdinalNumber),
                MovieSeries ms => (ms.ParentSeries, ms.OrdinalNumber ?? 0),
                _ => default
            };

            return movieSeries != null && (movieSeries.Entries.Any(e => e.OrdinalNumber > ordinalNumber) ||
                movieSeries.Parts.Any(p => p.OrdinalNumber! > ordinalNumber));
        }

        private async Task SelectPreviousEntryAsync(EntityBase? entity)
        {
            (MovieSeries? movieSeries, int ordinalNumber) = entity switch
            {
                MovieSeriesEntry entry => (entry.MovieSeries, entry.OrdinalNumber),
                MovieSeries ms => (ms.ParentSeries, ms.OrdinalNumber.Value),
                _ => default
            };

            if (movieSeries == null)
            {
                return;
            }

            var previousEntry = movieSeries.Entries
                .Where(e => e.OrdinalNumber < ordinalNumber)
                .OrderByDescending(e => e.OrdinalNumber)
                .FirstOrDefault();

            if (previousEntry != null && previousEntry.OrdinalNumber == ordinalNumber - 1)
            {
                await this.OpenEntryAsync(previousEntry);
            } else
            {
                var previousPart = movieSeries.Parts
                    .Where(p => p.OrdinalNumber < ordinalNumber)
                    .OrderByDescending(p => p.OrdinalNumber)
                    .FirstOrDefault();

                if (previousPart != null)
                {
                    await this.OpenEntryAsync(previousPart.GetFirstEntry());
                }
            }
        }

        private bool CanSelectPreviousEntry(EntityBase? entity)
        {
            (MovieSeries? movieSeries, int ordinalNumber) = entity switch
            {
                MovieSeriesEntry entry => (entry.MovieSeries, entry.OrdinalNumber),
                MovieSeries ms => (ms.ParentSeries, ms.OrdinalNumber ?? 0),
                _ => default
            };

            return movieSeries != null && (movieSeries.Entries.Any(e => e.OrdinalNumber < ordinalNumber) ||
                movieSeries.Parts.Any(p => p.OrdinalNumber! < ordinalNumber));
        }

        private async Task OpenEntryAsync(MovieSeriesEntry entry)
            => await (entry.Movie != null ? this.OpenMovieAsync(entry.Movie) : this.OpenSeriesAsync(entry.Series!));
    }
}
