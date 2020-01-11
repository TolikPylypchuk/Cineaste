using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels.Forms.Base
{
    public abstract class SeriesComponentFormBase<TModel, TViewModel>
        : TitledFormBase<TModel, TViewModel>, ISeriesComponentForm
        where TModel : class
        where TViewModel : SeriesComponentFormBase<TModel, TViewModel>
    {
        protected SeriesComponentFormBase(
            SeriesFormViewModel parent,
            IObservable<int> maxSequenceNumber,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Parent = parent;

            this.GoToSeries = ReactiveCommand.Create<Unit, SeriesFormViewModel>(_ => this.Parent, this.Valid);

            var isNotFirst = this.WhenAnyValue(vm => vm.SequenceNumber)
                .Select(num => num != 1);

            var isNotLast = Observable.CombineLatest(this.WhenAnyValue(vm => vm.SequenceNumber), maxSequenceNumber)
                .Select(nums => nums[0] < nums[1]);

            this.MoveUp = ReactiveCommand.Create(() => { this.SequenceNumber--; }, isNotFirst);
            this.MoveDown = ReactiveCommand.Create(() => { this.SequenceNumber++; }, isNotLast);

            this.SelectNext = ReactiveCommand.Create(
                () => { },
                Observable.CombineLatest(isNotLast, this.Valid).AllTrue());

            this.SelectPrevious = ReactiveCommand.Create(
                () => { },
                Observable.CombineLatest(isNotFirst, this.Valid).AllTrue());
        }

        public SeriesFormViewModel Parent { get; }

        public abstract string Channel { get; set; }

        [Reactive]
        public int SequenceNumber { get; set; }

        public ReactiveCommand<Unit, SeriesFormViewModel> GoToSeries { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        public ReactiveCommand<Unit, Unit> SelectNext { get; }
        public ReactiveCommand<Unit, Unit> SelectPrevious { get; }

        public abstract int GetNextYear();
    }
}
