using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using MovieList.Data.Models;
using MovieList.ViewModels;

namespace MovieList.Views
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
            => this.InitializeComponent();

        public SettingsViewModel ViewModel { get; set; }

        private void SettingsControl_Loaded(object sender, RoutedEventArgs e)
            => Task.Factory.StartNew(this.ViewModel.LoadKindsAsync);

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.IsLoaded)
            {
                this.ViewModel.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.ViewModel.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void AddKind_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Kinds.Add(new KindViewModel(new Kind
            {
                Name = Properties.Resources.NewKind,
                ColorForMovie = Colors.Black.ToString(),
                ColorForSeries = Colors.Black.ToString()
            }));

            this.ViewModel.KindsChanged = true;
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemoveKind_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is KindViewModel kind)
            {
                this.ViewModel.Kinds.Remove(kind);
                this.ViewModel.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void ViewLog_Click(object sender, RoutedEventArgs e)
            => this.ViewModel.ViewLog();

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.IsLoaded && this.ViewModel.CanSaveChanges();

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveChangesAsync().ContinueWith(success =>
            {
                if (success.Result)
                {
                    CommandManager.InvalidateRequerySuggested();
                } else
                {
                    MessageBox.Show(
                        Properties.Resources.SavingSettingsFailed,
                        Properties.Resources.Error,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.IsLoaded && this.ViewModel.CanCancelChanges();

        private async void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await this.ViewModel.CancelChangesAsync();
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
