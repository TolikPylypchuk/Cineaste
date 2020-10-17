using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using MovieList.Converters;
using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class FilterItemControlBase : ReactiveUserControl<FilterItemViewModel> { }

    public partial class FilterItemControl : FilterItemControlBase
    {
        private readonly FilterOperationConverter opConverter = new FilterOperationConverter();

        public FilterItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeTitle);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeYear);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeKind);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeTags);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeStandalone);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeMovie);
                this.FilterTypeComboBox.Items.Add(Messages.FilterTypeSeries);

                this.Bind(this.ViewModel, vm => vm.FilterType, v => v.FilterTypeComboBox.SelectedItem)
                    ?.DisposeWith(disposables);

                this.ViewModel!.AvailableOperations
                    .ToObservableChangeSet()
                    .Transform(this.FilterOperationToString)
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

        private string FilterOperationToString(FilterOperation op)
        {
            opConverter.TryConvert(op, typeof(string), null, out object? result);
            return result as string ?? String.Empty;
        }
    }
}
