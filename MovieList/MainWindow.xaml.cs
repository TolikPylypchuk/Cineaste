using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList
{
    public abstract class MainWindowBase : ReactiveWindow<MainViewModel> { }

    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }
}
