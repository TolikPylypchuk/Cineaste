namespace Cineaste.Views.Filters;

public partial class TextFilterInputControl : ReactiveUserControl<TextFilterInputViewModel>
{
    public TextFilterInputControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.Text, v => v.InputBox.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Description)
                .Select(description => Messages.ResourceManager.GetString(
                    $"FilterDescription{description}", CultureInfo.CurrentUICulture))
                .WhereNotNull()
                .BindTo(this, v => v.CaptionTextBlock.Text)
                .DisposeWith(disposables);
        });
    }
}
