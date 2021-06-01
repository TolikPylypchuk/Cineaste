using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using DynamicData.Aggregation;
using DynamicData.Binding;

using MovieList.Core.Data.Models;
using MovieList.Core.ViewModels.Forms.Base;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.Core.ViewModels.Forms
{
    public sealed class FranchiseEntryViewModel : ReactiveForm<FranchiseEntry, FranchiseEntryViewModel>
    {
        public FranchiseEntryViewModel(
            FranchiseEntry entry,
            FranchiseFormViewModel parentForm,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Log().Debug($"Creating a view model for an entry inside the franchise form: {entry}");

            this.Entry = entry;
            this.ParentForm = parentForm;

            this.Select = ReactiveCommand.Create(() => { });

            var canMoveUp = this.WhenAnyValue(vm => vm.SequenceNumber)
                .Select(num => num != 1);

            var canMoveDown = this.WhenAnyValue(vm => vm.SequenceNumber)
                .Select(num => num < this.Entry.ParentFranchise.Entries.Count);

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

        public FranchiseEntry Entry { get; }

        public FranchiseFormViewModel ParentForm { get; }

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

        public override bool IsNew =>
            this.Entry.Id == default;

        protected override FranchiseEntryViewModel Self => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.SequenceNumber, vm => vm.Entry.SequenceNumber);
            this.TrackChanges(vm => vm.DisplayNumber, vm => vm.Entry.DisplayNumber);

            base.EnableChangeTracking();
        }

        protected override IObservable<FranchiseEntry> OnSave()
        {
            this.Entry.SequenceNumber = this.SequenceNumber;
            this.Entry.DisplayNumber = this.DisplayNumber;

            return Observable.Return(this.Entry);
        }

        protected override IObservable<FranchiseEntry?> OnDelete() =>
            Observable.Return(this.Entry);

        protected override void CopyProperties()
        {
            this.Title = this.Entry.GetTitle()?.Name ?? String.Empty;
            this.Years = this.Entry.GetYears();
            this.SequenceNumber = this.Entry.SequenceNumber;
            this.DisplayNumber = this.Entry.DisplayNumber;

            if (this.Years == "0")
            {
                this.Years = String.Empty;
            }
        }
    }
}
