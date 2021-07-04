using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Filters;
using Cineaste.Properties;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public partial class SelectionFilterInputControl : ReactiveUserControl<SelectionFilterInputViewModel>
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

                this.OneWayBind(this.ViewModel!, vm => vm.Items, v => v.InputBox.Items)
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
}
