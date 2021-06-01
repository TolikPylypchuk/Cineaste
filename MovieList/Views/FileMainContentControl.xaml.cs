using System.Reactive.Disposables;

using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
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
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.List, v => v.ListViewHost.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.SideViewModel, v => v.SideViewHost.ViewModel)
                    .DisposeWith(disposables);
            });
        }
    }
}
