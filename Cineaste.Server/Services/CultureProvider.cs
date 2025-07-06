using System.Globalization;

namespace Cineaste.Server.Services;

public sealed class CultureProvider
{
    public List<SimpleCultureModel> GetAllCultures() =>
        [.. CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(culture => new SimpleCultureModel(culture.ToString(), culture.EnglishName))];
}
