using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Filters;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Views.Filters
{
    public partial class SimpleFilterItemControl : ReactiveUserControl<SimpleFilterItemViewModel>
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

                this.OtherActionsButton.GetObservable(Button.ClickEvent)
                    .Select(_ => this.OtherActionsButton.ContextMenu)
                    .WhereNotNull()
                    .Subscribe(menu => menu.Open())
                    .DisposeWith(disposables);
            });
        }

        private void BindFilter(CompositeDisposable disposables)
        {
            this.FilterTypeComboBox.SetEnumValues<FilterType>();

            this.Bind(this.ViewModel, vm => vm.FilterType, v => v.FilterTypeComboBox.SelectedItem)
                .DisposeWith(disposables);

            var filterOperationConverter = GetDefaultService<IEnumConverter<FilterOperation>>();

            this.ViewModel!.AvailableOperations
                .ToObservableChangeSet()
                .Transform(filterOperationConverter.ToString)
                .ToCollection()
                .Subscribe(ops => this.FilterOperationComboBox.Items = ops.ToList())
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.FilterOperation, v => v.FilterOperationComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                .Select(op => op != FilterOperation.None)
                .BindTo(this, v => v.FilterOperationComboBox.IsVisible)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel!, vm => vm.IsNegated, v => v.NegateCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel!, vm => vm.IsNegated, v => v.ColorStripRectangle.IsVisible)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                .Select(op => op != FilterOperation.None)
                .BindTo(this, v => v.FilterOperationCaption.IsVisible)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.FilterOperation)
                .Select(op => op != FilterOperation.None ? 2 : 1)
                .Subscribe(gridRow => Grid.SetRow(this.NegateGrid, gridRow))
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.FilterInput, v => v.InputViewHost.ViewModel)
                .DisposeWith(disposables);
        }

        private void BindCommands(CompositeDisposable disposables)
        {
            this.BindCommand(this.ViewModel, vm => vm.Delete, v => v.RemoveFilterItem)
                .DisposeWith(disposables);

            this.BindCommand(
                this.ViewModel,
                vm => vm.MakeComposite,
                v => v.MakeCompositeAndItem,
                Observable.Return(FilterComposition.And))
                .DisposeWith(disposables);

            this.BindCommand(
                this.ViewModel,
                vm => vm.MakeComposite,
                v => v.MakeCompositeOrItem,
                Observable.Return(FilterComposition.Or))
                .DisposeWith(disposables);
        }
    }
}
