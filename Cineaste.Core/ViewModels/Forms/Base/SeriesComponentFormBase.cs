using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.ViewModels.Forms.Base
{
    public abstract class SeriesComponentFormBase<TModel, TForm> : TitledFormBase<TModel, TForm>, ISeriesComponentForm
        where TModel : class
        where TForm : SeriesComponentFormBase<TModel, TForm>
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

            this.GoToNext = ReactiveCommand.Create(
                () => { },
                Observable.CombineLatest(isNotLast, this.Valid).AllTrue());

            this.GoToPrevious = ReactiveCommand.Create(
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

        public ReactiveCommand<Unit, Unit> GoToNext { get; }
        public ReactiveCommand<Unit, Unit> GoToPrevious { get; }

        public abstract int GetNextYear();
    }
}
