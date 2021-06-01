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
    public abstract class NumberFilterInputControlBase : ReactiveUserControl<NumberFilterInputViewModel> { }

    public partial class NumberFilterInputControl : NumberFilterInputControlBase
    {
        public NumberFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.InputBox.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Number, v => v.InputBox.Number)
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
