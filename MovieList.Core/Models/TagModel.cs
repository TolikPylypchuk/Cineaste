using System;
using System.Collections.ObjectModel;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.Models
{
    public sealed class TagModel : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; } = String.Empty;

        [Reactive]
        public string Description { get; set; } = String.Empty;

        [Reactive]
        public string Category { get; set; } = String.Empty;

        [Reactive]
        public string Color { get; set; } = String.Empty;

        public ObservableCollection<TagModel> ImpliedTags { get; } = new();
    }
}
