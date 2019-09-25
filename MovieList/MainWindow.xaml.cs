using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

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

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.ViewModel.OpenFile
                    .ObserveOnDispatcher()
                    .Subscribe(this.OnOpenFile)
                    .DisposeWith(disposables);
            });
        }

        private void OnOpenFile(string file)
        {
            if (!this.IsVisible)
            {
                this.Show();
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }
    }
}
