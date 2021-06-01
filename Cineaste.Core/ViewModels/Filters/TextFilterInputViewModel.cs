using System;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class TextFilterInputViewModel : FilterInput
    {
        public TextFilterInputViewModel() =>
            this.WhenAnyValue(vm => vm.Text)
                .Discard()
                .Subscribe(this.inputChanged);

        [Reactive]
        public string Text { get; set; } = String.Empty;
    }
}
