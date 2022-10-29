namespace Cineaste.Shared.Validation.TestData;

internal class RottenTomatoesIdTestData : TestDataBase
{
    public RottenTomatoesIdTestData()
    {
        // string? imdbId, bool isValid
        this.Add(null, true);
        this.Add(String.Empty, true);
        this.Add("m/movie", true);
        this.Add("m/some_other_movie", true);
        this.Add("tv/movie", true);
        this.Add("tv/some_other_movie", true);
        this.Add("name/movie", false);
        this.Add("name/some_other_movie", false);
        this.Add("https://www.rottentomatoes.com/m/movie", false);
        this.Add("https://www.rottentomatoes.com/tv/movie", false);
    }
}
