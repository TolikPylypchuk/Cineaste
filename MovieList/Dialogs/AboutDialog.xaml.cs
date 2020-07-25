using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.DialogModels;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Dialogs
{
    public abstract class AboutDialogBase : ReactiveUserControl<AboutModel> { }

    public partial class AboutDialog : AboutDialogBase
    {
        public AboutDialog()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.Version)
                    .Select(version => String.Format(Messages.AboutTextFormat, version))
                    .BindTo(this, v => v.AboutTextBlock.Text)
                    .DisposeWith(disposables);

                this.DocsLink.NavigateUri = new Uri(Messages.DocsLink, UriKind.Absolute);
            });
        }
    }
}
