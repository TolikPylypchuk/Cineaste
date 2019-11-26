using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using MovieList.ViewModels.Forms;

using ReactiveUI;

using Splat;

using static MovieList.Constants;

namespace MovieList.Views.Forms
{
    public abstract class MovieFormControlBase : ReactiveUserControl<MovieFormViewModel> { }

    public partial class MovieFormControl : MovieFormControlBase
    {
        private readonly IBlobCache cache;

        public MovieFormControl()
        {
            this.InitializeComponent();

            this.cache = Locator.Current.GetService<IBlobCache>(CacheKey);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.ViewModel.FormTitle
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Save, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                    .DisposeWith(disposables);

                Observable.CombineLatest(this.ViewModel.Save.CanExecute, this.ViewModel.Cancel.CanExecute)
                    .AnyTrue()
                    .Select(canSaveOrCancel => canSaveOrCancel.ToVisibility())
                    .BindTo(this, v => v.ActionPanel.Visibility);

                this.LoadPoster();

                this.ViewModel.Save.Subscribe(_ => this.LoadPoster());

                this.Bind(this.ViewModel, vm => vm.IsWatched, v => v.IsWatchedCheckBox.IsChecked)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.IsReleased, v => v.IsReleasedCheckBox.IsChecked)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.Year)
                    .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                    .WhereValueNotNull()
                    .Select(year => year == DateTime.Now.Year)
                    .BindTo(this, v => v.IsReleasedCheckBox.IsEnabled);

                this.WhenAnyValue(v => v.ViewModel.Year)
                    .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                    .WhereValueNotNull()
                    .Select(year => year <= DateTime.Now.Year)
                    .BindTo(this, v => v.IsWatchedCheckBox.IsEnabled);

                this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.AddTitle, v => v.AddTitleButton)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Year, v => v.YearTextBox.Text)
                    .DisposeWith(disposables);

                this.YearTextBox.ValidateWith(this.ViewModel.YearRule)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Kind, v => v.KindComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.KindComboBox.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLink.NavigateUri)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkRun.Text)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.ImdbLink, v => v.ImdbLinkTextBox.Text)
                    .DisposeWith(disposables);

                this.ImdbLinkTextBox.ValidateWith(this.ViewModel.ImdbLinkRule)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                    .DisposeWith(disposables);

                this.PosterUrlTextBox.ValidateWith(this.ViewModel.PosterUrlRule)
                    .DisposeWith(disposables);
            });
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel.PosterUrl))
            {
                this.cache.DownloadUrl(this.ViewModel.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Poster.Source);
            }
        }
    }
}
