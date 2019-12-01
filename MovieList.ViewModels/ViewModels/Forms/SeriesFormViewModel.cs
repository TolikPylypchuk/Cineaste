using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeriesFormViewModel : FormViewModelBase<Series, SeriesFormViewModel>
    {
        private readonly SourceList<Title> titlesSource;

        private readonly ReadOnlyObservableCollection<TitleFormViewModel> titles;
        private readonly ReadOnlyObservableCollection<TitleFormViewModel> originalTitles;

        public SeriesFormViewModel(
            Series series,
            ReadOnlyObservableCollection<Kind> kinds,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Series = series;
            this.Kinds = kinds;

            this.titlesSource = new SourceList<Title>();

            this.InitializeTitles(title => !title.IsOriginal, out this.titles);
            this.InitializeTitles(title => title.IsOriginal, out this.originalTitles);

            this.CopyProperties();

            this.FormTitle = this.CreateFormTitle();

            this.ImdbLinkRule = this.CreateImdbLinkRule();
            this.PosterUrlRule = this.CreatePosterUrlRule();

            Observable.Return(this.Series.Id != default)
                .Merge(this.Save.Select(_ => true))
                .Subscribe(this.CanDeleteSubject);

            var canAddTitle = this.Titles.ToObservableChangeSet()
                .Select(_ => this.Titles.Count < MaxTitleCount);

            var canAddOriginalTitle = this.OriginalTitles.ToObservableChangeSet()
                .Select(_ => this.OriginalTitles.Count < MaxTitleCount);

            this.AddTitle = ReactiveCommand.Create(() => this.OnAddTitle(false), canAddTitle);
            this.AddOriginalTitle = ReactiveCommand.Create(() => this.OnAddTitle(true), canAddOriginalTitle);
            this.Close = ReactiveCommand.Create(() => { });

            this.InitializeChangeTracking();
        }

        public Series Series { get; }

        public IObservable<string> FormTitle { get; }

        public ReadOnlyObservableCollection<Kind> Kinds { get; }

        public ReadOnlyObservableCollection<TitleFormViewModel> Titles
            => this.titles;

        public ReadOnlyObservableCollection<TitleFormViewModel> OriginalTitles
            => this.originalTitles;

        [Reactive]
        public Kind Kind { get; set; } = null!;

        [Reactive]
        public bool IsWatched { get; set; }

        [Reactive]
        public bool IsAnthology { get; set; }

        [Reactive]
        public SeriesStatus Status { get; set; }

        [Reactive]
        public string ImdbLink { get; set; } = String.Empty;

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        public ValidationHelper ImdbLinkRule { get; }
        public ValidationHelper PosterUrlRule { get; }

        public ReactiveCommand<Unit, Unit> AddTitle { get; }
        public ReactiveCommand<Unit, Unit> AddOriginalTitle { get; }
        public ReactiveCommand<Unit, Unit> Close { get; }

        protected override void InitializeChangeTracking()
        {
        }

        protected override Task<Series?> OnDeleteAsync()
            => Task.FromResult<Series?>(null);

        protected override Task<Series> OnSaveAsync()
            => Task.FromResult(this.Series);

        protected override void CopyProperties()
        {
        }

        private void InitializeTitles(
            Func<Title, bool> predicate,
            out ReadOnlyObservableCollection<TitleFormViewModel> titles)
        {
            var canDelete = this.titlesSource.Connect()
                .Select(_ => this.titlesSource.Items.Where(predicate).Count())
                .Select(count => count > 1);

            this.titlesSource.Connect()
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

            titleForm.Delete.Subscribe(_ =>
            {
                this.titlesSource.Remove(title);

                (!title.IsOriginal ? this.Titles : this.OriginalTitles)
                    .Where(t => t.Priority > title.Priority)
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
                .Select(title => this.Series.Id != default || !String.IsNullOrWhiteSpace(title)
                    ? title
                    : this.ResourceManager.GetString("NewSeries") ?? String.Empty);

        private ValidationHelper CreateImdbLinkRule()
            => this.ValidationRule(
                vm => vm.ImdbLink,
                link => link.IsUrl(),
                this.ResourceManager.GetString("ValidationImdbLinkInvalid"));

        private ValidationHelper CreatePosterUrlRule()
            => this.ValidationRule(
                vm => vm.PosterUrl,
                url => url.IsUrl(),
                this.ResourceManager.GetString("ValidationPosterUrlInvalid"));

        private void OnAddTitle(bool isOriginal)
            => this.titlesSource.Add(new Title
            {
                IsOriginal = isOriginal,
                Series = this.Series,
                Priority = !isOriginal ? this.Titles.Count + 1 : this.OriginalTitles.Count + 1
            });

        private bool AreTitlesChanged(IReadOnlyCollection<TitleFormViewModel> vms)
            => vms.Count != this.Series.Titles.Count(title => !title.IsOriginal) ||
               vms.Any(vm => vm.IsFormChanged || this.Series.Id != default && vm.Title.Id == default);
    }
}
