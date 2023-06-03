namespace Cineaste.Server.Services;

using System.Globalization;

public sealed class CultureExtractor
{
    public List<SimpleCultureModel> GetAllCultures() =>
        CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(culture => new SimpleCultureModel(culture.ToString(), culture.EnglishName))
            .ToList();
}
