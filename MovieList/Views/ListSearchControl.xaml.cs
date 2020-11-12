using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Core.ViewModels;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListSearchControlBase : ReactiveUserControl<ListSearchViewModel> { }

    public partial class ListSearchControl : ListSearchControlBase
    {
        public ListSearchControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.FilterItem, v => v.FilterItemViewHost.ViewModel)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.FindNext, v => v.FindNextButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.FindPrevious, v => v.FindPreviousButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.StopSearch, v => v.StopSearchButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Clear, v => v.ClearSearchButton)
                    .DisposeWith(disposables);

                this.OneWayBind(
                    this.ViewModel!, vm => vm.IsSearchInitialized, v => v.FoundItemsCountTextBlock.Visibility)
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                    this.ViewModel!.FoundItems.ToObservableChangeSet().Count(),
                    this.WhenAnyValue(v => v.ViewModel!.CurrentIndex),
                    this.WhenAnyValue(v => v.ViewModel!.TotalSearchedItemsCount),
                    this.FoundItemsCountMessage)
                    .BindTo(this, v => v.FoundItemsCountTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.SearchResults, v => v.SearchResults.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.CurrentResult, v => v.SearchResults.SelectedItem)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.SearchResults.SelectedItem)
                    .Subscribe(this.SearchResults.ScrollIntoView)
                    .DisposeWith(disposables);
            });
        }

        private string FoundItemsCountMessage(int numItems, int currentIndex, int numAllItems) =>
            (numItems, numAllItems) switch
            {
                (0, _) => Messages.NoItemsFound,
                (int n1, int n2) when n1 == n2 => Messages.AllItemsFound,
                _ => String.Format(Messages.SearchResultFormat, currentIndex + 1, numItems)
            };
    }
}
