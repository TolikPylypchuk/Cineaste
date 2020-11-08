using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

using MovieList.Core.Comparers;
using MovieList.Core.Data.Models;
using MovieList.Core.ListItems;
using MovieList.Data;

using Nito.Comparers;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels
{
    public enum ListSorting
    {
        ByTitle,
        ByOriginalTitle,
        ByTitleSimple,
        ByOriginalTitleSimple,
        ByYear
    }

    public sealed class ListSortViewModel : ReactiveObject
    {
        private readonly Settings settings;
        private readonly IComparer<string> simpleTitleComparer;

        public ListSortViewModel(Settings settings)
        {
            this.settings = settings;
            this.simpleTitleComparer = new TitleComparer(this.settings.CultureInfo);

            this.Apply = ReactiveCommand.Create(this.OnApply);

            this.Apply.ToPropertyEx(this, vm => vm.Comparer, initialValue: this.OnApply());
        }

        [Reactive]
        public ListSorting FirstSorting { get; set; } = ListSorting.ByTitle;

        [Reactive]
        public ListSortDirection FirstDirection { get; set; } = ListSortDirection.Ascending;

        [Reactive]
        public ListSorting SecondSorting { get; set; } = ListSorting.ByYear;

        [Reactive]
        public ListSortDirection SecondDirection { get; set; } = ListSortDirection.Ascending;

        public IComparer<ListItem> Comparer { [ObservableAsProperty] get; } = null!;

        public ReactiveCommand<Unit, IComparer<ListItem>> Apply { get; }

        private IComparer<ListItem> OnApply()
        {
            var secondComparer = this.CreateSecondComparer();
            var firstComparer = this.CreateFirstComparer(secondComparer);

            return ComparerBuilder.For<ListItem>()
                .OrderBy(
                    item => item,
                    firstComparer,
                    descending: this.FirstDirection == ListSortDirection.Descending)
                .ThenBy(item => item, secondComparer, descending: false); // add false to disable ambiguity
        }

        private IComparer<ListItem> CreateFirstComparer(IComparer<ListItem> secondComparer) =>
            this.FirstSorting switch
            {
                ListSorting.ByTitle => new ListItemTitleComparer(
                    this.settings.CultureInfo,
                    secondComparer,
                    item => item.Title,
                    franchise => franchise.GetListTitle()?.Name ?? String.Empty),

                ListSorting.ByOriginalTitle => new ListItemTitleComparer(
                    this.settings.CultureInfo,
                    secondComparer,
                    item => item.OriginalTitle,
                    franchise => franchise.GetListOriginalTitle()?.Name ?? String.Empty),

                ListSorting.ByTitleSimple =>
                    ComparerBuilder.For<ListItem>().OrderBy(item => item.Title, this.simpleTitleComparer),

                ListSorting.ByOriginalTitleSimple =>
                    ComparerBuilder.For<ListItem>().OrderBy(item => item.OriginalTitle, this.simpleTitleComparer),

                ListSorting.ByYear =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(item => item.StartYearToCompare)
                        .ThenBy(item => item.EndYearToCompare, descending: false), // add false to disable ambiguity

                _ => throw new InvalidOperationException($"Invlid first sorting: {this.FirstSorting}")
            };

        private IComparer<ListItem> CreateSecondComparer() =>
            this.SecondSorting switch
            {
                ListSorting.ByTitleSimple =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.Title,
                            this.simpleTitleComparer,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                ListSorting.ByOriginalTitleSimple =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.OriginalTitle,
                            this.simpleTitleComparer,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                ListSorting.ByYear =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.StartYearToCompare,
                            descending: this.SecondDirection == ListSortDirection.Descending)
                        .ThenBy(
                            item => item.EndYearToCompare,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                _ => throw new InvalidOperationException($"Invlid second sorting: {this.SecondSorting}")
            };
    }
}
