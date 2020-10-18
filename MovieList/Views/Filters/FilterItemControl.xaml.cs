using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core;
using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

using Splat;

namespace MovieList.Views.Filters
{
    public abstract class FilterItemControlBase : ReactiveUserControl<FilterItemViewModel> { }

    public partial class FilterItemControl : FilterItemControlBase
    {
        public FilterItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.Bind(this.ViewModel!, vm => vm.IsNegated, v => v.NegateCheckBox.IsChecked)
                    ?.DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel!,
                    vm => vm.IsNegated,
                    v => v.ColorStripRectangle.Visibility,
                    BooleanToVisibilityHint.UseHidden)
                    ?.DisposeWith(disposables);

                this.FilterTypeComboBox.AddEnumValues<FilterType>();

                this.Bind(this.ViewModel, vm => vm.FilterType, v => v.FilterTypeComboBox.SelectedItem)
                    ?.DisposeWith(disposables);

                var filterOperationConverter = Locator.Current.GetService<IEnumConverter<FilterOperation>>();

                this.ViewModel!.AvailableOperations
                    .ToObservableChangeSet()
                    .Transform(filterOperationConverter.ToString)
                    .ToCollection()
                    .Subscribe(ops =>
                    {
                        this.FilterOperationComboBox.Items.Clear();
                        ops.ForEach(op => this.FilterOperationComboBox.Items.Add(op));
                    })
                    ?.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.FilterOperation, v => v.FilterOperationComboBox.SelectedItem)
                    ?.DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                    .Select(op => op != FilterOperation.None)
                    .BindTo(this, v => v.FilterOperationComboBox.Visibility)
                    ?.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.FilterInput, v => v.InputViewHost.ViewModel)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
