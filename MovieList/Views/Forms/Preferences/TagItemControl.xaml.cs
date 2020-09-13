using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

using MovieList.Core;
using MovieList.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Views.Forms.Preferences
{
    public abstract class TagItemControlBase : ReactiveUserControl<TagItemViewModel> { }

    public partial class TagItemControl : TagItemControlBase
    {
        public TagItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Name, v => v.Chip.Content)
                    ?.DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Color, v => v.Chip.IconBackground)
                    ?.DisposeWith(disposables);

                Observable.CombineLatest(
                    this.WhenAnyValue(v => v.ViewModel!.Category),
                    this.WhenAnyValue(v => v.ViewModel!.Description),
                    (category, description) => $"{category} | {description}")
                    .BindTo(this, v => v.Chip.ToolTip)
                    ?.DisposeWith(disposables);

                if (this.ViewModel!.CanSelect)
                {
                    this.Chip.Cursor = Cursors.Hand;

                    Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                        h => this.Chip.Click += h,
                        h => this.Chip.Click -= h)
                        .Discard()
                        .InvokeCommand(this.ViewModel.Select)
                        ?.DisposeWith(disposables);
                }

                if (this.ViewModel.CanDelete)
                {
                    this.Chip.IsDeletable = true;

                    Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                        h => this.Chip.DeleteClick += h,
                        h => this.Chip.DeleteClick -= h)
                        .Discard()
                        .InvokeCommand(this.ViewModel.Delete)
                        ?.DisposeWith(disposables);
                }
            });
        }
    }
}
