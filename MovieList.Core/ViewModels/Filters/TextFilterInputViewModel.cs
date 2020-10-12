using System;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class TextFilterInputViewModel : FilterInput
    {
        [Reactive]
        public string Text { get; set; } = String.Empty;
    }
}
