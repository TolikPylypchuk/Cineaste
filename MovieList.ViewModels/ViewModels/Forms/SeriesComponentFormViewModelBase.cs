using System.Reactive;
using System.Reactive.Concurrency;
using System.Resources;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels.Forms
{
    public abstract class SeriesComponentFormViewModelBase<TModel, TViewModel> : TitledFormViewModelBase<TModel, TViewModel>
        where TModel : class
        where TViewModel : SeriesComponentFormViewModelBase<TModel, TViewModel>
    {
        protected SeriesComponentFormViewModelBase(
            SeriesFormViewModel parent,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Parent = parent;

            this.GoToSeries = ReactiveCommand.Create<Unit, SeriesFormViewModel>(_ => this.Parent);

            this.MoveUp = ReactiveCommand.Create(() => { this.SequenceNumber--; });
            this.MoveDown = ReactiveCommand.Create(() => { this.SequenceNumber++; });
        }

        public SeriesFormViewModel Parent { get; }

        [Reactive]
        public int SequenceNumber { get; set; }

        public ReactiveCommand<Unit, SeriesFormViewModel> GoToSeries { get; }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }
    }
}
