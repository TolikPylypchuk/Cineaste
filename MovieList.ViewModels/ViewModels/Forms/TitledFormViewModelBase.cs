using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;

using ReactiveUI;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public abstract class TitledFormViewModelBase<TModel, TViewModel> : FormViewModelBase<TModel, TViewModel>
        where TModel : class
        where TViewModel : TitledFormViewModelBase<TModel, TViewModel>
    {
        private readonly ReadOnlyObservableCollection<TitleFormViewModel> titles;
        private readonly ReadOnlyObservableCollection<TitleFormViewModel> originalTitles;

        protected TitledFormViewModelBase(ResourceManager? resourceManager, IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.TitlesSource = new SourceList<Title>();

            this.InitializeTitles(title => !title.IsOriginal, out this.titles);
            this.InitializeTitles(title => title.IsOriginal, out this.originalTitles);

            this.FormTitle = this.CreateFormTitle();

            var canAddTitle = this.Titles.ToObservableChangeSet()
                .Select(_ => this.Titles.Count < MaxTitleCount);

            var canAddOriginalTitle = this.OriginalTitles.ToObservableChangeSet()
                .Select(_ => this.OriginalTitles.Count < MaxTitleCount);

            this.AddTitle = ReactiveCommand.Create(() => this.OnAddTitle(false), canAddTitle);
            this.AddOriginalTitle = ReactiveCommand.Create(() => this.OnAddTitle(true), canAddOriginalTitle);

            this.TitlesChanged = this.AreTitlesChanged(this.Titles, "Titles");
            this.OriginalTitlesChanged = this.AreTitlesChanged(this.OriginalTitles, "Original titles");

            this.TitlesValid = this.AreTitlesValid(this.Titles);
            this.OriginalTitlesValid = this.AreTitlesValid(this.OriginalTitles);
        }

        public IObservable<string> FormTitle { get; }

        public ReadOnlyObservableCollection<TitleFormViewModel> Titles
            => this.titles;

        public ReadOnlyObservableCollection<TitleFormViewModel> OriginalTitles
            => this.originalTitles;

        public ReactiveCommand<Unit, Unit> AddTitle { get; }
        public ReactiveCommand<Unit, Unit> AddOriginalTitle { get; }

        protected SourceList<Title> TitlesSource { get; }

        protected IObservable<bool> TitlesChanged { get; }
        protected IObservable<bool> OriginalTitlesChanged { get; }

        protected IObservable<bool> TitlesValid { get; }
        protected IObservable<bool> OriginalTitlesValid { get; }

        protected abstract IEnumerable<Title> ItemTitles { get; }

        protected abstract string NewItemKey { get; }

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(this.AreTitlesChanged(this.Titles, "Titles"));
            this.TrackChanges(this.AreTitlesChanged(this.OriginalTitles, "Original titles"));

            this.TrackValidation(this.AreTitlesValid(this.Titles));
            this.TrackValidation(this.AreTitlesValid(this.OriginalTitles));

            base.EnableChangeTracking();
        }

        protected abstract void AttachTitle(Title title);

        private void InitializeTitles(
            Func<Title, bool> predicate,
            out ReadOnlyObservableCollection<TitleFormViewModel> titles)
        {
            var canDelete = this.TitlesSource.Connect()
                .Select(_ => this.TitlesSource.Items.Where(predicate).Count())
                .Select(count => count > 1);

            this.TitlesSource.Connect()
                .Filter(predicate)
                .Sort(SortExpressionComparer<Title>.Ascending(title => title.Priority))
                .Transform(title => this.CreateTitleForm(title, canDelete))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out titles)
                .DisposeMany()
                .Subscribe();
        }

        private TitleFormViewModel CreateTitleForm(Title title, IObservable<bool> canDelete)
        {
            var titleForm = new TitleFormViewModel(title, canDelete, this.ResourceManager);

            titleForm.Delete
                .WhereNotNull()
                .Subscribe(deletedTitle =>
                {
                    this.TitlesSource.Remove(deletedTitle);

                    (!deletedTitle.IsOriginal ? this.Titles : this.OriginalTitles)
                        .Where(t => t.Priority > deletedTitle.Priority)
                        .ForEach(t => t.Priority--);
                });

            return titleForm;
        }

        private IObservable<string> CreateFormTitle()
            => this.Titles.ToObservableChangeSet()
                .AutoRefresh(vm => vm.Name)
                .AutoRefresh(vm => vm.Priority)
                .ToCollection()
                .Select(vms => vms.OrderBy(vm => vm.Priority).Select(vm => vm.Name).FirstOrDefault())
                .Select(title => this.IsNew && String.IsNullOrWhiteSpace(title)
                    ? this.ResourceManager.GetString(this.NewItemKey) ?? String.Empty
                    : title);

        private IObservable<bool> AreTitlesChanged(
            ReadOnlyObservableCollection<TitleFormViewModel> titles,
            string description)
            => titles.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.FormChanged)
                .ToCollection()
                .Select(vms =>
                    vms.Count == 0 ||
                    vms.Count != this.ItemTitles.Count(title => title.IsOriginal == vms.First().Title.IsOriginal) ||
                    vms.Any(vm => vm.IsFormChanged || !this.IsNew && vm.IsNew))
                .Do(changed => this.Log().Debug(
                    changed ? $"{description} are changed" : $"{description} are unchanged"));

        private IObservable<bool> AreTitlesValid(ReadOnlyObservableCollection<TitleFormViewModel> titles)
            => titles.ToObservableChangeSet()
                .AutoRefreshOnObservable(vm => vm.Valid)
                .ToCollection()
                .SelectMany(vms => vms.Select(vm => vm.Valid).CombineLatest().AllTrue());

        private void OnAddTitle(bool isOriginal)
        {
            var title = new Title
            {
                IsOriginal = isOriginal,
                Priority = !isOriginal ? this.Titles.Count + 1 : this.OriginalTitles.Count + 1
            };

            this.AttachTitle(title);
            this.TitlesSource.Add(title);
        }
    }
}
