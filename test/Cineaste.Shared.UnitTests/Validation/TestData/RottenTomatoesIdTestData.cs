using System.Collections;

namespace Cineaste.Shared.Validation.TestData;

internal class RottenTomatoesIdTestData : IEnumerable<TheoryDataRow<string?, bool>>
{
    // string? rottenTomatoesId, bool isValid
    public IEnumerator<TheoryDataRow<string?, bool>> GetEnumerator()
    {
        yield return new(null, true);
        yield return new(String.Empty, true);
        yield return new("m/movie", true);
        yield return new("m/some_other_movie", true);
        yield return new("tv/movie", true);
        yield return new("tv/some_other_movie", true);
        yield return new("name/movie", false);
        yield return new("name/some_other_movie", false);
        yield return new("https://www.rottentomatoes.com/m/movie", false);
        yield return new("https://www.rottentomatoes.com/tv/movie", false);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
