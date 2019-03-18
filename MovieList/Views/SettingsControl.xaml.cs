using System.Windows.Controls;

using MovieList.ViewModels;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Views
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
