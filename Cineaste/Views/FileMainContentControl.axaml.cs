namespace Cineaste.Views;

public partial class FileMainContentControl : ReactiveUserControl<FileMainContentViewModel>
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
