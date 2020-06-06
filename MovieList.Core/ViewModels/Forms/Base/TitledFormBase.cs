using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Data.Models;

using ReactiveUI;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms.Base
{
    public abstract class TitledFormBase<TModel, TViewModel> : ReactiveForm<TModel, TViewModel>
        where TModel : class
        where TViewModel : TitledFormBase<TModel, TViewModel>
    {
        private readonly SourceList<Title> titlesSource = new SourceList<Title>();

        private readonly ReadOnlyObservableCollection<TitleFormViewModel> titles;
        private readonly ReadOnlyObservableCollection<TitleFormViewModel> originalTitles;

        protected TitledFormBase(ResourceManager? resourceManager, IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.InitializeTitles(title => !title.IsOriginal, out this.titles);
            this.InitializeTitles(title => title.IsOriginal, out this.originalTitles);

            this.FormTitle = this.Titles.ToObservableChangeSet()
                .AutoRefresh(vm => vm.Name)
                .AutoRefresh(vm => vm.Priority)
                .ToCollection()
                .Select(vms => vms.OrderBy(vm => vm.Priority).Select(vm => vm.Name).FirstOrDefault())
                .Select(this.GetFormTitle)
                .ObserveOn(RxApp.MainThreadScheduler);

            var canAddTitle = this.Titles.ToObservableChangeSet()
                .Count()
                .Select(count => count < MaxTitleCount);

            var canAddOriginalTitle = this.OriginalTitles.ToObservableChangeSet()
                .Count()
                .Select(count => count < MaxTitleCount);

            this.Close = ReactiveCommand.Create(() => { });

            this.AddTitle = ReactiveCommand.Create(() => this.OnAddTitle(false), canAddTitle);
            this.AddOriginalTitle = ReactiveCommand.Create(() => this.OnAddTitle(true), canAddOriginalTitle);
        }

        public IObservable<string> FormTitle { get; protected set; }

        public ReadOnlyObservableCollection<TitleFormViewModel> Titles
            => this.titles;

        public ReadOnlyObservableCollection<TitleFormViewModel> OriginalTitles
            => this.originalTitles;

        public ReactiveCommand<Unit, Unit> Close { get; }

        public ReactiveCommand<Unit, Unit> AddTitle { get; }
        public ReactiveCommand<Unit, Unit> AddOriginalTitle { get; }

        protected abstract ICollection<Title> ItemTitles { get; }

        protected abstract string NewItemKey { get; }

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(this.IsCollectionChanged(
                vm => vm.Titles,
                vm => vm.ItemTitles.Where(title => !title.IsOriginal).ToList()));

            this.TrackChanges(this.IsCollectionChanged(
                vm => vm.OriginalTitles,
                vm => vm.ItemTitles.Where(title => title.IsOriginal).ToList()));

            this.TrackValidation(this.IsCollectionValid(this.Titles));
            this.TrackValidation(this.IsCollectionValid(this.OriginalTitles));

            base.EnableChangeTracking();
        }

        protected override void CopyProperties()
            => this.titlesSource.Edit(titles =>
            {
                titles.Clear();
                titles.AddRange(this.ItemTitles);
            });

        protected abstract void AttachTitle(Title title);

        protected IObservable<Unit> SaveTitles()
        {
            var allTitles = this.Titles.Union(this.OriginalTitles).ToList();

            return allTitles.Count == 0
                ? Observable.Return(Unit.Default)
                : allTitles
                    .Select(title => title.Save.Execute())
                    .ForkJoin()
                    .Discard()
                    .Do(() =>
                    {
                        foreach (var title in this.titlesSource.Items.Except(this.ItemTitles).ToList())
                        {
                            this.ItemTitles.Add(title);
                        }

                        foreach (var title in this.ItemTitles.Except(this.titlesSource.Items).ToList())
                        {
                            this.ItemTitles.Remove(title);
                        }
                    });
        }

        protected void ClearTitles()
            => this.titlesSource.Clear();

        protected string GetFormTitle(string title)
            => this.IsNew && String.IsNullOrWhiteSpace(title)
                ? this.ResourceManager.GetString(this.NewItemKey) ?? String.Empty
                : title;

        private void InitializeTitles(
            Func<Title, bool> predicate,
            out ReadOnlyObservableCollection<TitleFormViewModel> titles)
        {
            var canDelete = this.titlesSource.Connect()
                .Select(_ => this.titlesSource.Items.Where(predicate).Count())
                .Select(count => count > MinTitleCount);

            this.titlesSource.Connect()
                .Filter(predicate)
                .Sort(SortExpressionComparer<Title>.Ascending(title => title.Priority))
                .Transform(title => this.CreateTitleForm(title, canDelete))
                .Bind(out titles)
                .DisposeMany()
                .Subscribe();
        }

        private TitleFormViewModel CreateTitleForm(Title title, IObservable<bool> canDelete)
        {
            var titleForm = new TitleFormViewModel(title, canDelete, this.ResourceManager, this.Scheduler);

            titleForm.Delete
                .WhereNotNull()
                .Subscribe(deletedTitle =>
                {
                    this.titlesSource.Remove(deletedTitle);

                    (!deletedTitle.IsOriginal ? this.Titles : this.OriginalTitles)
                        .Where(t => t.Priority > deletedTitle.Priority)
                        .ForEach(t => t.Priority--);
                });

            return titleForm;
        }

        private void OnAddTitle(bool isOriginal)
        {
            var title = new Title
            {
                IsOriginal = isOriginal,
                Priority = !isOriginal ? this.Titles.Count + 1 : this.OriginalTitles.Count + 1
            };

            this.AttachTitle(title);
            this.titlesSource.Add(title);
        }
    }
}
