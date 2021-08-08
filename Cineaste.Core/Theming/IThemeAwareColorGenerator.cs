using System;

namespace Cineaste.Core.Theming
{
    public interface IThemeAwareColorGenerator
    {
        string TransformColorForCurrentTheme(string color);

        IObservable<string> CreateThemeAwareColor(string color);
    }
}
