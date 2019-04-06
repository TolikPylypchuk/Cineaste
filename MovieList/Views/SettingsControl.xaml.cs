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

        private void SettingsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
            => Task.Factory.StartNew(this.ViewModel.LoadKindsAsync);

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.OldValue != null)
            {
                this.ViewModel.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ViewModel.KindsChanged = true;
            CommandManager.InvalidateRequerySuggested();
        }

        private void AddKind_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Kinds.Add(new Kind
            {
                Name = Properties.Resources.NewKind,
                ColorForMovie = "Black",
                ColorForSeries = "Black"
            });

            this.ViewModel.KindsChanged = true;
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemoveKind_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Kind kind)
            {
                this.ViewModel.Kinds.Remove(kind);
                this.ViewModel.KindsChanged = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.ViewModel?.CanSaveChanges() ?? false;

        private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await this.ViewModel.SaveChangesAsync();
            CommandManager.InvalidateRequerySuggested();
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = this.ViewModel?.CanSaveChanges() ?? false;

        private async void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await this.ViewModel.CancelChangesAsync();
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
