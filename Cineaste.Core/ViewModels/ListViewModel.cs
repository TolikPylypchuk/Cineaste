using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Cineaste.Core.Data;
using Cineaste.Core.Data.Models;
using Cineaste.Core.ListItems;
using Cineaste.Data;
using Cineaste.Data.Models;
using Cineaste.Data.Services;

using DynamicData;
using DynamicData.Binding;

using Nito.Comparers;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Core.ViewModels
{
    public sealed class ListViewModel : ReactiveObject
    {
        private readonly SourceCache<ListItem, string> source = new(item => item.Id);
        private readonly ReadOnlyObservableCollection<ListItemViewModel> items;
        private readonly Subject<Unit> resort = new();

        public ListViewModel(
            string fileName,
            IObservable<ListItemViewModel> find,
            IObservable<Func<ListItem, bool>> filter,
            IObservable<IComparer<ListItem>> comparer,
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            Settings? settings = null,
            IListService? listService = null)
        {
            settings ??= GetDefaultService<Settings>(fileName);
            listService ??= GetDefaultService<IListService>(fileName);

            this.Find = find;

            this.DataList = listService.GetList(kinds, tags);
            this.source.AddOrUpdate(this.DataList.ToListItems());
            this.Log().Debug($"Loaded the list of {this.source.Count} items");

            this.source.Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Filter(item => !String.IsNullOrEmpty(item.Title))
                .Filter(filter.ObserveOn(RxApp.TaskpoolScheduler))
                .AutoRefreshOnObservable(item => item.WhenAnyPropertyChanged())
                .Transform(item => new ListItemViewModel(item))
                .Sort(
                    comparer
                        .Select(c => ComparerBuilder.For<ListItemViewModel>().OrderBy(vm => vm.Item, c))
                        .ObserveOn(RxApp.TaskpoolScheduler),
                    this.resort)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.items)
                .Subscribe();

            this.SelectItem = ReactiveCommand.Create<ListItem?, bool>(this.OnSelectItem);
            this.PreviewSelectItem = ReactiveCommand.Create<ListItemViewModel, ListItemViewModel>(vm => vm);
            this.ForceSelectedItem = ReactiveCommand.Create(() => { });
            this.AddOrUpdate = ReactiveCommand.Create<ListItem, ListItem>(this.OnAddOrUpdate);
            this.RemoveMovie = ReactiveCommand.Create<Movie>(this.OnRemoveMovie);
            this.RemoveSeries = ReactiveCommand.Create<Series>(this.OnRemoveSeries);
            this.RemoveFranchise = ReactiveCommand.Create<Franchise>(this.OnRemoveFranchise);

            this.AddOrUpdate.InvokeCommand(this.SelectItem);
        }

        public ReadOnlyObservableCollection<ListItemViewModel> Items =>
            this.items;

        [Reactive]
        public DataList DataList { get; private set; }

        [Reactive]
        public ListItemViewModel? SelectedItem { get; set; }

        public ReactiveCommand<ListItem?, bool> SelectItem { get; }
        public ReactiveCommand<ListItemViewModel, ListItemViewModel> PreviewSelectItem { get; }
        public ReactiveCommand<Unit, Unit> ForceSelectedItem { get; }

        public ReactiveCommand<ListItem, ListItem> AddOrUpdate { get; }

        public ReactiveCommand<Movie, Unit> RemoveMovie { get; }
        public ReactiveCommand<Series, Unit> RemoveSeries { get; }
        public ReactiveCommand<Franchise, Unit> RemoveFranchise { get; }

        public IObservable<ListItemViewModel> Find { get; }

        private bool OnSelectItem(ListItem? item)
        {
            bool isSame = this.SelectedItem?.Item.Id == item?.Id;

            this.SetSelectedForSelectedItem(false);
            this.SelectedItem = this.Items.FirstOrDefault(vm => vm.Item == item);
            this.SetSelectedForSelectedItem(true);

            return !isSame || item == null;
        }

        private ListItem OnAddOrUpdate(ListItem item)
        {
            this.source.Edit(list =>
            {
                list.AddOrUpdate(item);

                var franchise = this.GetFranchise(item);

                if (franchise != null)
                {
                    list.AddOrUpdate(new FranchiseListItem(franchise));

                    franchise.Entries
                        .Select(entry => entry.ToListItem())
                        .ForEach(list.AddOrUpdate);

                    this.UpdateParentFranchise(franchise, list);
                }
            });

            this.Resort();

            return item;
        }

        private Franchise? GetFranchise(ListItem item) =>
            item switch
            {
                MovieListItem movieItem when movieItem.Movie.Entry != null => movieItem.Movie.Entry.ParentFranchise,
                SeriesListItem seriesItem when seriesItem.Series.Entry != null => seriesItem.Series.Entry.ParentFranchise,
                FranchiseListItem franchiseItem => franchiseItem.Franchise,
                _ => null
            };

        private void OnRemoveMovie(Movie movie)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new MovieListItem(movie).Id);

                if (movie.Entry != null)
                {
                    this.RemoveFranchiseEntry(movie.Entry, list);
                }
            });

            this.Resort();
        }

        private void OnRemoveSeries(Series series)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new SeriesListItem(series).Id);

                if (series.Entry != null)
                {
                    this.RemoveFranchiseEntry(series.Entry, list);
                }
            });

            this.Resort();
        }

        private void OnRemoveFranchise(Franchise franchise)
        {
            this.source.Edit(list =>
            {
                list.RemoveKey(new FranchiseListItem(franchise).Id);

                if (franchise.Entry != null)
                {
                    this.RemoveFranchiseEntry(franchise.Entry, list);
                }

                foreach (var entry in franchise.Entries)
                {
                    list.AddOrUpdate(entry.ToListItem());
                }
            });

            this.Resort();
        }

        private void RemoveFranchiseEntry(FranchiseEntry franchiseEntry, ISourceUpdater<ListItem, string> list)
        {
            var franchise = franchiseEntry.ParentFranchise;

            franchise.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber < franchiseEntry.SequenceNumber)
                .Select(entry => entry.ToListItem())
                .ForEach(list.AddOrUpdate);

            this.UpdateParentFranchise(franchise, list);

            if (franchise.Entries.Count == 0 && franchise.ShowTitles)
            {
                list.RemoveKey(new FranchiseListItem(franchise).Id);

                if (franchise.Entry != null)
                {
                    this.RemoveFranchiseEntry(franchise.Entry, list);
                }
            }
        }

        private void Resort() =>
            this.resort.OnNext(Unit.Default);

        private void UpdateParentFranchise(Franchise franchise, ISourceUpdater<ListItem, string> list)
        {
            franchise.Entry?.ParentFranchise.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber <= franchise.Entry.SequenceNumber)
                .TakeWhile(entry => entry.Franchise != null && entry.Franchise.MergeDisplayNumbers)
                .SelectMany(entry => entry.Franchise!.Entries)
                .Select(entry => entry.ToListItem())
                .ForEach(list.AddOrUpdate);
        }

        private void SetSelectedForSelectedItem(bool isSelected)
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItem.Item.IsSelected = isSelected;
            }
        }
    }
}
