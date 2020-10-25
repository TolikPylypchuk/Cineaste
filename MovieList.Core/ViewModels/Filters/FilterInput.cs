using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public abstract class FilterInput : ReactiveObject
    {
        protected Subject<Unit> inputChanged = new();

        private protected FilterInput()
        { }

        [Reactive]
        public string Description { get; set; } = String.Empty;

        public IObservable<Unit> InputChanged
            => this.inputChanged.AsObservable();
    }
}
