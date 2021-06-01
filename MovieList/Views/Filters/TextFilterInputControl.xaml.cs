using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Filters;
using Cineaste.Properties;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public abstract class TextFilterInputControlBase : ReactiveUserControl<TextFilterInputViewModel> { }

    public partial class TextFilterInputControl : TextFilterInputControlBase
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
                    .Select(description => Messages.ResourceManager.GetString($"FilterDescription{description}"))
                    .WhereNotNull()
                    .Subscribe(description => HintAssist.SetHint(this.InputBox, description))
                    .DisposeWith(disposables);
            });
        }
    }
}
