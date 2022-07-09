namespace Cineaste.Server.Services;

using System.Globalization;

[GenerateAutoInterface]
public sealed class CultureExtractor : ICultureExtractor
{
    public List<SimpleCultureModel> GetAllCultures() =>
        CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(culture => new SimpleCultureModel(culture.ToString(), culture.EnglishName))
            .ToList();
}
