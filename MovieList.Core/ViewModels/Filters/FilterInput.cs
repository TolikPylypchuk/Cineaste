using System;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public abstract class FilterInput : ReactiveObject
    {
        private protected FilterInput()
        { }

        [Reactive]
        public string Description { get; set; } = String.Empty;
    }
}
