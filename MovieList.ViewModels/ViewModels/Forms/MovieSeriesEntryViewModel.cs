using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Data.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class MovieSeriesEntryViewModel : FormBase<MovieSeriesEntry, MovieSeriesEntryViewModel>
    {
        public MovieSeriesEntryViewModel(
            MovieSeriesEntry entry,
            MovieSeriesFormViewModel parentForm,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Log().Debug($"Creating a view model for an entry inside the movie series form: {entry}");

            this.Entry = entry;
            this.ParentForm = parentForm;

            this.Select = ReactiveCommand.Create(() => { });

            var canMoveUp = this.WhenAnyValue(vm => vm.SequenceNumber)
                .Select(num => num != 1);

            var canMoveDown = this.WhenAnyValue(vm => vm.SequenceNumber)
                .Select(num => num < this.Entry.ParentSeries.Entries.Count);

            this.MoveUp = ReactiveCommand.Create(() => { }, canMoveUp);
            this.MoveDown = ReactiveCommand.Create(() => { }, canMoveDown);

            var canHideDisplayNumber = this.WhenAnyValue(vm => vm.ParentForm.IsLooselyConnected)
                .Invert()
                .CombineLatest(this.WhenAnyValue(vm => vm.DisplayNumber).Select(num => num != null), (a, b) => a && b);

            var canShowDisplayNumber = this.WhenAnyValue(vm => vm.ParentForm.IsLooselyConnected)
                .Invert()
                .CombineLatest(this.WhenAnyValue(vm => vm.DisplayNumber).Select(num => num == null), (a, b) => a && b);

            this.HideDisplayNumber = ReactiveCommand.Create(() => { }, canHideDisplayNumber);
            this.ShowDisplayNumber = ReactiveCommand.Create(() => { }, canShowDisplayNumber);

            this.WhenAnyValue(
                    vm => vm.DisplayNumber,
                    vm => vm.ParentForm.IsLooselyConnected,
                    (num, isLooselyConnected) => (Number: num, IsLooselyConnected: isLooselyConnected))
                .Select(props => props.Number.AsDisplayNumber(props.IsLooselyConnected))
                .ToPropertyEx(this, vm => vm.NumberToDisplay);

            this.CopyProperties();

            this.CanDeleteWhen(this.ParentForm.Entries.ToObservableChangeSet().Count().Select(count => count > 1));

            this.EnableChangeTracking();
        }

        public MovieSeriesEntry Entry { get; }

        public MovieSeriesFormViewModel ParentForm { get; }

        public string Title { get; private set; } = String.Empty;
        public string Years { get; private set; } = String.Empty;

        [Reactive]
        public int SequenceNumber { get; set; }

        [Reactive]
        public int? DisplayNumber { get; set; }

        public string NumberToDisplay { [ObservableAsProperty] get; } = String.Empty;

        public ReactiveCommand<Unit, Unit> Select { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        public ReactiveCommand<Unit, Unit> HideDisplayNumber { get; }
        public ReactiveCommand<Unit, Unit> ShowDisplayNumber { get; }

        public override bool IsNew
            => this.Entry.Id == default;

        protected override MovieSeriesEntryViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.SequenceNumber, vm => vm.Entry.SequenceNumber);
            this.TrackChanges(vm => vm.DisplayNumber, vm => vm.Entry.DisplayNumber);

            base.EnableChangeTracking();
        }

        protected override Task<MovieSeriesEntry> OnSaveAsync()
        {
            this.Entry.SequenceNumber = this.SequenceNumber;
            this.Entry.DisplayNumber = this.DisplayNumber;

            return Task.FromResult(this.Entry);
        }

        [SuppressMessage("ReSharper", "RedundantCast")]
        protected override Task<MovieSeriesEntry?> OnDeleteAsync()
            => Task.FromResult((MovieSeriesEntry?)this.Entry);

        protected override void CopyProperties()
        {
            this.Title = this.Entry.GetTitle()?.Name ?? String.Empty;
            this.Years = this.Entry.GetYears();
            this.SequenceNumber = this.Entry.SequenceNumber;
            this.DisplayNumber = this.Entry.DisplayNumber;
        }
    }
}
