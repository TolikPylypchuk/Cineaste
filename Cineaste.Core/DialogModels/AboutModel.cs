using System;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.DialogModels
{
    public sealed class AboutModel : ReactiveObject
    {
        [Reactive]
        public string Version { get; set; } = String.Empty;
    }
}
