using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Data;

using ReactiveUI;

using static Cineaste.Data.ListSortOrder;

namespace Cineaste.Views.Forms.Preferences
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

                this.DefaultFirstSortOrderComboBox.AddEnumValues<ListSortOrder>();
                this.DefaultFirstSortDirectionComboBox.AddEnumValues<ListSortDirection>();
                this.DefaultSecondSortOrderComboBox.AddEnumValues(ByTitleSimple, ByOriginalTitleSimple, ByYear);
                this.DefaultSecondSortDirectionComboBox.AddEnumValues<ListSortDirection>();

                this.BindFields(disposables);
                this.BindCommands(disposables);
            });
        }

        private void BindFields(CompositeDisposable disposables)
        {
            this.CultureInfoComboBox.ItemsSource = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .OrderBy(culture => culture.EnglishName)
                .ToList();

            this.Bind(this.ViewModel, vm => vm.ListName, v => v.ListNameTextBox.Text)
                .DisposeWith(disposables);

            this.ListNameTextBox.ValidateWith(this.ViewModel!.ListNameRule);

            this.Bind(this.ViewModel, vm => vm.CultureInfo, v => v.CultureInfoComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.DefaultFirstSortOrder, v => v.DefaultFirstSortOrderComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(
                this.ViewModel,
                vm => vm.DefaultFirstSortDirection,
                v => v.DefaultFirstSortDirectionComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.DefaultSeasonTitle, v => v.DefaultSeasonTitleTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(
                    this.ViewModel,
                    vm => vm.DefaultSeasonOriginalTitle,
                    v => v.DefaultSeasonOriginalTitleTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(
                this.ViewModel, vm => vm.DefaultSecondSortOrder, v => v.DefaultSecondSortOrderComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(
                this.ViewModel,
                vm => vm.DefaultSecondSortDirection,
                v => v.DefaultSecondSortDirectionComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.Kinds, v => v.Kinds.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.TagItems, v => v.Tags.ItemsSource)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.AddKind, v => v.AddKindButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.AddTag, v => v.AddTagButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Save, v => v.SaveButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel!, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(disposables);

            Observable.CombineLatest(this.ViewModel!.Save.CanExecute, this.ViewModel.Cancel.CanExecute)
                .AnyTrue()
                .BindTo(this, v => v.ActionPanel.Visibility)
                .DisposeWith(disposables);
        }
    }
}
