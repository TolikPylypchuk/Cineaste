using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

using Cineaste.Core.Comparers;
using Cineaste.Core.Data.Models;
using Cineaste.Core.ListItems;
using Cineaste.Data;

using Nito.Comparers;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.ViewModels
{
    public sealed class ListSortViewModel : ReactiveObject
    {
        private readonly Settings settings;
        private readonly IComparer<string> simpleTitleComparer;

        public ListSortViewModel(Settings settings)
        {
            this.settings = settings;
            this.simpleTitleComparer = new TitleComparer(this.settings.CultureInfo);

            this.FirstOrder = settings.DefaultFirstSortOrder;
            this.SecondOrder = settings.DefaultSecondSortOrder;

            this.FirstDirection = settings.DefaultFirstSortDirection;
            this.SecondDirection = settings.DefaultSecondSortDirection;

            this.Apply = ReactiveCommand.Create(this.OnApply);

            this.Apply.ToPropertyEx(this, vm => vm.Comparer, initialValue: this.OnApply());
        }

        [Reactive]
        public ListSortOrder FirstOrder { get; set; }

        [Reactive]
        public ListSortOrder SecondOrder { get; set; }

        [Reactive]
        public ListSortDirection FirstDirection { get; set; }

        [Reactive]
        public ListSortDirection SecondDirection { get; set; }

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
                .ThenBy(item => item, secondComparer, descending: false); // add false to remove ambiguity
        }

        private IComparer<ListItem> CreateFirstComparer(IComparer<ListItem> secondComparer) =>
            this.FirstOrder switch
            {
                ListSortOrder.ByTitle => new ListItemTitleComparer(
                    this.settings.CultureInfo,
                    secondComparer,
                    item => item.Title,
                    franchise => franchise.GetListTitle()?.Name ?? String.Empty),

                ListSortOrder.ByOriginalTitle => new ListItemTitleComparer(
                    this.settings.CultureInfo,
                    secondComparer,
                    item => item.OriginalTitle,
                    franchise => franchise.GetListOriginalTitle()?.Name ?? String.Empty),

                ListSortOrder.ByTitleSimple =>
                    ComparerBuilder.For<ListItem>().OrderBy(item => item.Title, this.simpleTitleComparer),

                ListSortOrder.ByOriginalTitleSimple =>
                    ComparerBuilder.For<ListItem>().OrderBy(item => item.OriginalTitle, this.simpleTitleComparer),

                ListSortOrder.ByYear =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(item => item.StartYearToCompare)
                        .ThenBy(item => item.EndYearToCompare, descending: false), // add false to remove ambiguity

                _ => throw new InvalidOperationException($"Invalid first sort order: {this.FirstOrder}")
            };

        private IComparer<ListItem> CreateSecondComparer() =>
            this.SecondOrder switch
            {
                ListSortOrder.ByTitleSimple =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.Title,
                            this.simpleTitleComparer,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                ListSortOrder.ByOriginalTitleSimple =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.OriginalTitle,
                            this.simpleTitleComparer,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                ListSortOrder.ByYear =>
                    ComparerBuilder.For<ListItem>()
                        .OrderBy(
                            item => item.StartYearToCompare,
                            descending: this.SecondDirection == ListSortDirection.Descending)
                        .ThenBy(
                            item => item.EndYearToCompare,
                            descending: this.SecondDirection == ListSortDirection.Descending),

                _ => throw new InvalidOperationException($"Invalid second sort order: {this.SecondOrder}")
            };
    }
}
