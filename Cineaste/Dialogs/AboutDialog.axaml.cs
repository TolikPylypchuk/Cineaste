namespace Cineaste.Dialogs;

public partial class AboutDialog : ReactiveUserControl<AboutModel>
{
    public AboutDialog()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.Version)
                .Select(version => String.Format(CultureInfo.CurrentCulture, Messages.AboutTextFormat, version))
                .BindTo(this, v => v.AboutTextBlock.Text)
                .DisposeWith(disposables);

            this.DocsButton.NavigateUri = new Uri(Messages.DocsLink, UriKind.Absolute);

            this.OKButton.GetObservable(Button.ClickEvent)
                .Discard()
                .InvokeCommand(this.ViewModel!.Close)
                .DisposeWith(disposables);
        });
    }
}
