using System.Reactive.Disposables;

using Cineaste.Core.ViewModels.Filters;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public abstract class NoFilterInputControlBase : ReactiveUserControl<NoFilterInputViewModel> { }

    public partial class NoFilterInputControl : NoFilterInputControlBase
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
}
