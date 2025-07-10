using System.Collections;

namespace Cineaste.Shared.Validation.TestData;

internal class ImdbIdTestData : IEnumerable<TheoryDataRow<string?, bool>>
{
    // string? imdbId, bool isValid
    public IEnumerator<TheoryDataRow<string?, bool>> GetEnumerator()
    {
        yield return new(null, true);
        yield return new(String.Empty, true);
        yield return new("tt1", true);
        yield return new("tt12345678", true);
        yield return new("abcd", false);
        yield return new("nm12345678", false);
        yield return new("https://www.imdb.com/tt12345678", false);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
