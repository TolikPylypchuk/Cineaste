namespace Cineaste.Views;

public partial class TabHeaderControl : ReactiveUserControl<TabHeaderViewModel>
{
    public TabHeaderControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.TabName, v => v.NameTextBlock.Text)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Close, v => v.CloseButton)
                .DisposeWith(disposables);
        });
    }
}
