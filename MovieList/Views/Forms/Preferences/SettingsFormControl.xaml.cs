using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class SettingsFormControlBase : ReactiveUserControl<SettingsFormViewModel> { }

    public partial class SettingsFormControl : SettingsFormControlBase
    {
        public SettingsFormControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindFields(disposables);
                this.BindCommands(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.Kinds.ItemsSource)
                    .DisposeWith(disposables);
            });
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.CultureInfoComboBox.ItemsSource = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .OrderBy(culture => culture.EnglishName)
                .ToList();

            this.Bind(this.ViewModel, vm => vm.ListName, v => v.ListNameTextBox.Text)
                .DisposeWith(disposables);

            this.ListNameTextBox.ValidateWith(this.ViewModel.ListNameRule);

            this.Bind(this.ViewModel, vm => vm.CultureInfo, v => v.CultureInfoComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.DefaultSeasonTitle, v => v.DefaultSeasonTitleTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(
                    this.ViewModel,
                    vm => vm.DefaultSeasonOriginalTitle,
                    v => v.DefaultSeasonOriginalTitleTextBox.Text)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel, vm => vm.AddKind, v => v.AddKindButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                    this.WhenAnyObservable(v => v.ViewModel.Save.CanExecute),
                    this.WhenAnyObservable(v => v.ViewModel.Cancel.CanExecute))
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.Visibility)
                .DisposeWith(disposables);
        }
    }
}
