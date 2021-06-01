using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Filters;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Views.Filters
{
    public abstract class SimpleFilterItemControlBase : ReactiveUserControl<SimpleFilterItemViewModel> { }

    public partial class SimpleFilterItemControl : SimpleFilterItemControlBase
    {
        public SimpleFilterItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindFilter(disposables);
                this.BindCommands(disposables);
            });
        }

        private void BindFilter(CompositeDisposable disposables)
        {
            this.FilterTypeComboBox.AddEnumValues<FilterType>();

            this.Bind(this.ViewModel, vm => vm.FilterType, v => v.FilterTypeComboBox.SelectedItem)
                .DisposeWith(disposables);

            var filterOperationConverter = GetDefaultService<IEnumConverter<FilterOperation>>();

            this.ViewModel!.AvailableOperations
                .ToObservableChangeSet()
                .Transform(filterOperationConverter.ToString)
                .ToCollection()
                .Subscribe(ops =>
                {
                    this.FilterOperationComboBox.Items.Clear();
                    ops.ForEach(op => this.FilterOperationComboBox.Items.Add(op));
                })
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.FilterOperation, v => v.FilterOperationComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                .Select(op => op != FilterOperation.None)
                .BindTo(this, v => v.FilterOperationComboBox.Visibility)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel!, vm => vm.IsNegated, v => v.NegateCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(
                this.ViewModel!,
                vm => vm.IsNegated,
                v => v.ColorStripRectangle.Visibility,
                BooleanToVisibilityHint.UseHidden)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                .Select(op => op != FilterOperation.None ? 1 : 0)
                .Subscribe(gridRow => Grid.SetRow(this.NegateCheckBox, gridRow))
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.FilterInput, v => v.InputViewHost.ViewModel)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel!, vm => vm.Delete, v => v.RemoveFilterButton)
                .DisposeWith(disposables);

            this.BindCommand(
                this.ViewModel!,
                vm => vm.MakeComposite,
                v => v.MakeCompositeAndButton,
                Observable.Return(FilterComposition.And))
                .DisposeWith(disposables);

            this.BindCommand(
                this.ViewModel!,
                vm => vm.MakeComposite,
                v => v.MakeCompositeOrButton,
                Observable.Return(FilterComposition.Or))
                .DisposeWith(disposables);
        }
    }
}
