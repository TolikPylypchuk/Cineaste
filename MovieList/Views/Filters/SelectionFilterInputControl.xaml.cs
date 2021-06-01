using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Filters;
using Cineaste.Properties;

using DynamicData;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public abstract class SelectionFilterInputControlBase : ReactiveUserControl<SelectionFilterInputViewModel> { }

    public partial class SelectionFilterInputControl : SelectionFilterInputControlBase
    {
        public SelectionFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel!, vm => vm.SelectedItem, v => v.InputBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel!, vm => vm.Items, v => v.InputBox.ItemsSource)
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
