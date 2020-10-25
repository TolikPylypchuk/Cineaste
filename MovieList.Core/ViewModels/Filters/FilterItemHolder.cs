using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public abstract class FilterItemHolder : ReactiveObject
    {
        private protected FilterItemHolder(
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags)
        {
            this.Clear = ReactiveCommand.Create(() => { });

            this.Clear
                .StartWith(Unit.Default)
                .Select(() => this.WithSubscriptions(new SimpleFilterItemViewModel(kinds, tags)))
                .BindTo(this, vm => vm.FilterItem);
        }

        [Reactive]
        public FilterItem FilterItem { get; set; } = null!;

        public ReactiveCommand<Unit, Unit> Clear { get; }

        protected FilterItem WithSubscriptions(FilterItem filterItem)
            => filterItem switch
            {
                SimpleFilterItemViewModel simpleItem => this.WithSubscriptions(simpleItem),
                CompositeFilterItemViewModel simpleItem => this.WithSubscriptions(simpleItem),
                _ => filterItem
            };

        protected SimpleFilterItemViewModel WithSubscriptions(SimpleFilterItemViewModel filterItem)
        {
            var subscriptions = new CompositeDisposable();

            filterItem.Delete
                .InvokeCommand(this.Clear)
                .DisposeWith(subscriptions);

            filterItem.MakeComposite
                .Select(composition => this.WithSubscriptions(
                    CompositeFilterItemViewModel.FromSimpleItem(filterItem, composition)))
                .BindTo(this, vm => vm.FilterItem)
                .DisposeWith(subscriptions);

            filterItem.Delete
                .Merge(filterItem.MakeComposite.Discard())
                .Subscribe(subscriptions.Dispose)
                .DisposeWith(subscriptions);

            this.AddSubscriptions(filterItem, subscriptions);

            return filterItem;
        }

        protected CompositeFilterItemViewModel WithSubscriptions(CompositeFilterItemViewModel filterItem)
        {
            var subscriptions = new CompositeDisposable();

            filterItem.Delete
                .InvokeCommand(this.Clear)
                .DisposeWith(subscriptions);

            filterItem.Simplify
                .Select(this.WithSubscriptions)
                .BindTo(this, vm => vm.FilterItem)
                .DisposeWith(subscriptions);

            filterItem.Delete
                .Merge(filterItem.Simplify.Discard())
                .Subscribe(subscriptions.Dispose)
                .DisposeWith(subscriptions);

            this.AddSubscriptions(filterItem, subscriptions);

            return filterItem;
        }

        protected virtual void AddSubscriptions(
            SimpleFilterItemViewModel filterItem,
            CompositeDisposable subscriptions)
        { }

        protected virtual void AddSubscriptions(
            CompositeFilterItemViewModel filterItem,
            CompositeDisposable subscriptions)
        { }
    }
}
