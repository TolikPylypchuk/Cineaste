using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.DialogModels;
using Cineaste.Properties;

using ReactiveUI;

namespace Cineaste.Dialogs
{
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

                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Version)
                    .Select(version => String.Format(Messages.AboutTextFormat, version))
                    .BindTo(this, v => v.AboutTextBlock.Text)
                    .DisposeWith(disposables);

                this.OKButton.GetObservable(Button.ClickEvent)
                    .Discard()
                    .InvokeCommand(this.ViewModel!.Close)
                    .DisposeWith(disposables);

                this.DocsButton.GetObservable(Button.ClickEvent)
                    .Discard()
                    .Subscribe(() => new Uri(Messages.DocsLink, UriKind.Absolute).OpenInBrowser())
                    .DisposeWith(disposables);
            });
        }
    }
}
