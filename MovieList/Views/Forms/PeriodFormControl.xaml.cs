using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MovieList.Properties;
using MovieList.ViewModels.Forms;

using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace MovieList.Views.Forms
{
    public abstract class PeriodFormControlBase : ReactiveUserControl<PeriodFormViewModel> { }

    public partial class PeriodFormControl : PeriodFormControlBase
    {
        private readonly List<string> monthNames = new List<string>
        {
            Messages.January,
            Messages.February,
            Messages.March,
            Messages.April,
            Messages.May,
            Messages.June,
            Messages.July,
            Messages.August,
            Messages.September,
            Messages.October,
            Messages.November,
            Messages.December
        };

        public PeriodFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                foreach (string month in this.monthNames)
                {
                    this.StartMonthComboBox.Items.Add(month);
                    this.EndMonthComboBox.Items.Add(month);
                }

                this.Bind(
                        this.ViewModel,
                        vm => vm.StartMonth,
                        v => v.StartMonthComboBox.SelectedIndex,
                        month => month - 1,
                        index => index + 1)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.StartYear, v => v.StartYearTextBox.Text)
                    .DisposeWith(disposables);

                this.StartYearTextBox.ValidateWith(this.ViewModel.StartYearRule)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        vm => vm.EndMonth,
                        v => v.EndMonthComboBox.SelectedIndex,
                        month => month - 1,
                        index => index + 1)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.EndYear, v => v.EndYearTextBox.Text)
                    .DisposeWith(disposables);

                this.EndYearTextBox.ValidateWith(this.ViewModel.EndYearRule)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.NumberOfEpisodes, v => v.NumberOfEpisodesTextBox.Text)
                    .DisposeWith(disposables);

                this.NumberOfEpisodesTextBox.ValidateWith(this.ViewModel.NumberOfEpisodesRule)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.IsSingleDayRelease, v => v.IsSingleDayReleaseCheckBox.IsChecked)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.PosterUrl, v => v.PosterUrlTextBox.Text)
                    .DisposeWith(disposables);

                this.PosterUrlTextBox.ValidateWith(this.ViewModel.PosterUrlRule)
                    .DisposeWith(disposables);

                this.BindValidation(this.ViewModel, vm => vm.PeriodRule, v => v.InvalidFormTextBlock.Text)
                    .DisposeWith(disposables);

                var boolToVisibility = new BooleanToVisibilityTypeConverter();

                this.WhenAnyObservable(v => v.ViewModel.PeriodRule.ValidationChanged)
                    .Select(state => state.IsValid)
                    .BindTo(this, v => v.InvalidFormTextBlock.Visibility, null, boolToVisibility)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.DeleteButton)
                    .DisposeWith(disposables);

                this.WhenAnyObservable(v => v.ViewModel.Delete.CanExecute)
                    .BindTo(this, v => v.DeleteButton.Visibility, null, boolToVisibility)
                    .DisposeWith(disposables);
            });
        }
    }
}
