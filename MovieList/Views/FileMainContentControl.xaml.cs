using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class FileMainContentControlBase : ReactiveUserControl<FileMainContentViewModel> { }

    public partial class FileMainContentControl : FileMainContentControlBase
    {
        public FileMainContentControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.List, v => v.ListViewHost.ViewModel)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.SideViewModel, v => v.SideViewHost.ViewModel)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
