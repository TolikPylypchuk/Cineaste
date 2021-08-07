using Cineaste.Core.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.Theming
{
    public sealed class ThemeManager : ReactiveObject
    {
        public ThemeManager(Theme initialTheme) =>
            this.Theme = initialTheme;

        [Reactive]
        public Theme Theme { get; set; }
    }
}
