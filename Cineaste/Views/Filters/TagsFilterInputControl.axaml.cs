using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Filters;
using Cineaste.Core.ViewModels.Forms.Preferences;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public partial class TagsFilterInputControl : ReactiveUserControl<TagsFilterInputViewModel>
    {
        public TagsFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Tags, v => v.Tags.Items)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.AddableTags, v => v.AddableTagsComboBox.Items)
                    .DisposeWith(disposables);

                this.AddableTagsComboBox.GetObservable(SelectingItemsControl.SelectionChangedEvent)
                    .Select(e => e.AddedItems.OfType<AddableTagViewModel>().FirstOrDefault())
                    .WhereNotNull()
                    .Select(vm => vm.Tag)
                    .Do(_ => this.AddableTagsComboBox.SelectedItem = null)
                    .InvokeCommand(this.ViewModel!.AddTag)
                    .DisposeWith(disposables);

                this.ViewModel!.AddableTags
                    .ToObservableChangeSet()
                    .Count()
                    .StartWith(this.ViewModel.AddableTags.Count)
                    .Select(count => count > 0)
                    .BindTo(this, v => v.AddableTagsComboBox.IsEnabled)
                    .DisposeWith(disposables);
            });
        }
    }
}
