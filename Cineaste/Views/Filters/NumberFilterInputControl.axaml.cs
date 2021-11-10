namespace Cineaste.Views.Filters;

public partial class NumberFilterInputControl : ReactiveUserControl<NumberFilterInputViewModel>
{
    public NumberFilterInputControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Number, v => v.Input.Value)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Description)
                .Select(description => Messages.ResourceManager.GetString(
                    $"FilterDescription{description}", CultureInfo.CurrentCulture))
                .WhereNotNull()
                .BindTo(this, v => v.CaptionTextBlock.Text)
                .DisposeWith(disposables);
        });
    }
}
