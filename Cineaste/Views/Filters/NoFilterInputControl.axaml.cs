namespace Cineaste.Views.Filters;

public partial class NoFilterInputControl : ReactiveUserControl<NoFilterInputViewModel>
{
    public NoFilterInputControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);
        });
    }
}
