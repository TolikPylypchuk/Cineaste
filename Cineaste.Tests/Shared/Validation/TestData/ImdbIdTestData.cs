namespace Cineaste.Shared.Validation.TestData;

internal class ImdbIdTestData : TestDataBase
{
    public ImdbIdTestData()
    {
        // string? imdbId, bool isValid
        this.Add(null, true);
        this.Add(String.Empty, true);
        this.Add("tt1", true);
        this.Add("tt12345678", true);
        this.Add("abcd", false);
        this.Add("nm12345678", false);
        this.Add("https://www.imdb.com/tt12345678", false);
    }
}
