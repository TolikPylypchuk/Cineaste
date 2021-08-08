using System;
using System.Reactive.Linq;

using Cineaste.Core.Theming;

using FluentAvalonia.UI.Media;

using ReactiveUI;

namespace Cineaste.Theming
{
    public sealed class ThemeAwareColorGenerator : IThemeAwareColorGenerator
    {
        private readonly ThemeManager themeManager;

        public ThemeAwareColorGenerator(ThemeManager themeManager) =>
            this.themeManager = themeManager;

        public string TransformColorForCurrentTheme(string color) =>
            themeManager.Theme == Theme.Light ? color : this.TransformColorForDarkTheme(color);

        public IObservable<string> CreateThemeAwareColor(string color) =>
            this.themeManager.WhenAnyValue(tm => tm.Theme)
                .Select(theme => this.TransformColorForCurrentTheme(color));

        private string TransformColorForDarkTheme(string color) =>
            Color2.TryParse(color, out var color2)
                ? Color2.FromHSL(color2.Hue, color2.Saturation, 100 - color2.Lightness, color2.A)
                    .ToHexString()
                    .ToUpper()
                : color;
    }
}
