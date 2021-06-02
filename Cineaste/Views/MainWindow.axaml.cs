using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels;

namespace Cineaste.Views
{
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow() =>
            this.InitializeComponent();
    }
}
