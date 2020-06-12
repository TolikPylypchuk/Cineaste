using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Akavache;

using MovieList.ViewModels.Forms;

using ReactiveUI;

namespace MovieList.Views.Forms
{
    public abstract class SpecialEpisodeFormControlBase : ReactiveUserControl<SpecialEpisodeFormViewModel> { }

    public partial class SpecialEpisodeFormControl : SpecialEpisodeFormControlBase
    {
        public SpecialEpisodeFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                foreach (string month in Properties.MonthNames)
                {
                    this.MonthComboBox.Items.Add(month);
                }

                this.ViewModel.FormTitle
                    .BindTo(this, v => v.FormTitleTextBlock.Text)
                    .DisposeWith(disposables);

                this.LoadPoster();

                this.BindCommands(disposables);
                this.BindFields(disposables);
                this.BindCheckboxes(disposables);

                this.AddValidation(disposables);
            });
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            const BooleanToVisibilityHint useHidden = BooleanToVisibilityHint.UseHidden;

            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToSeries, v => v.GoToSeriesIconButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToSeries.CanExecute
                .BindTo(this, v => v.GoToSeriesIconButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            this.ViewModel.Cancel.CanExecute
                .BindTo(this, v => v.CancelButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToNext, v => v.GoToNextButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToNext.CanExecute
                .BindTo(this, v => v.GoToNextButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GoToPrevious, v => v.GoToPreviousButton)
                .DisposeWith(disposables);

            this.ViewModel.GoToPrevious.CanExecute
                .BindTo(this, v => v.GoToPreviousButton.Visibility, useHidden)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                .DisposeWith(disposables);

            this.ViewModel.Delete.CanExecute
                .BindTo(this, v => v.DeleteButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddTitle, v => v.AddTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddTitle.CanExecute
                .BindTo(this, v => v.AddTitleButton.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddOriginalTitle, v => v.AddOriginalTitleButton)
                .DisposeWith(disposables);

            this.ViewModel.AddOriginalTitle.CanExecute
                .BindTo(this, v => v.AddOriginalTitleButton.Visibility)
                .DisposeWith(disposables);

            this.ViewModel.Save
                .Subscribe(_ => this.LoadPoster())
                .DisposeWith(disposables);
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.OneWayBind(this.ViewModel, vm => vm.Titles, v => v.Titles.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.OriginalTitles, v => v.OriginalTitles.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Channel, v => v.ChannelTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(
                    this.ViewModel,
                    vm => vm.Month,
                    v => v.MonthComboBox.SelectedIndex,
                    month => month - 1,
                    index => index + 1)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Year, v => v.YearTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                .DisposeWith(disposables);
        }

        private void BindCheckboxes(CompositeDisposable disposables)
        {
            this.Bind(this.ViewModel, vm => vm.IsWatched, v => v.IsWatchedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.IsReleased, v => v.IsReleasedCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.Year)
                .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                .WhereValueNotNull()
                .CombineLatest(this.WhenAnyValue(v => v.ViewModel.Month), (year, month) => (Year: year, Month: month))
                .Select(values =>
                    values.Year < DateTime.Now.Year ||
                    values.Year == DateTime.Now.Year && values.Month <= DateTime.Now.Month)
                .ObserveOnDispatcher()
                .BindTo(this, v => v.IsWatchedCheckBox.IsEnabled)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.Year)
                .Select(year => Int32.TryParse(year, out int value) ? (int?)value : null)
                .WhereValueNotNull()
                .CombineLatest(this.WhenAnyValue(v => v.ViewModel.Month), (year, month) => (Year: year, Month: month))
                .Select(values => values.Month == DateTime.Now.Month && values.Year == DateTime.Now.Year)
                .BindTo(this, v => v.IsReleasedCheckBox.IsEnabled)
                .DisposeWith(disposables);
        }

        private void AddValidation(CompositeDisposable disposables)
        {
            this.ChannelTextBox.ValidateWith(this.ViewModel.ChannelRule)
                .DisposeWith(disposables);

            this.YearTextBox.ValidateWith(this.ViewModel.YearRule)
                .DisposeWith(disposables);

            this.PosterUrlTextBox.ValidateWith(this.ViewModel.PosterUrlRule)
                .DisposeWith(disposables);
        }

        private void LoadPoster()
        {
            if (!String.IsNullOrEmpty(this.ViewModel.PosterUrl))
            {
                BlobCache.UserAccount.DownloadUrl(this.ViewModel.PosterUrl, TimeSpan.FromMinutes(5))
                    .Select(data => data.AsImage())
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .BindTo(this, v => v.Poster.Source);
            } else
            {
                this.Poster.Source = null;
            }
        }
    }
}
