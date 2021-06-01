using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Cineaste.Converters;
using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Data;
using Cineaste.Properties;

using ReactiveUI;

using static Cineaste.Data.ListSortOrder;

namespace Cineaste.Views.Forms.Preferences
{
    public abstract class PreferencesFormControlBase : ReactiveUserControl<PreferencesFormViewModel> { }

    public partial class PreferencesFormControl : PreferencesFormControlBase
    {
        private static readonly LogLevelConverter LogLevelConverter = new LogLevelConverter();

        public PreferencesFormControl()
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

            this.Bind(this.ViewModel, vm => vm.ShowRecentFiles, v => v.ShowRecentFilesCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.LogPath, v => v.LogPathTextBox.Text)
                .DisposeWith(disposables);

            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelVerbose);
            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelDebug);
            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelInformation);
            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelWarning);
            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelError);
            this.MinLogLevelComboBox.Items.Add(Messages.LogLevelFatal);

            this.Bind(
                this.ViewModel,
                vm => vm.MinLogLevel,
                v => v.MinLogLevelComboBox.SelectedItem,
                null,
                LogLevelConverter,
                LogLevelConverter)
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
