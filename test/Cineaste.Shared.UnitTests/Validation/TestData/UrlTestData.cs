using System.Collections;

namespace Cineaste.Shared.Validation.TestData;

internal class UrlTestData : IEnumerable<TheoryDataRow<string, bool>>
{
    // string? url, bool isValid
    public IEnumerator<TheoryDataRow<string, bool>> GetEnumerator()
    {
        yield return new("123qwe", false);
        yield return new("www.tolik.io", false);
        yield return new("http://www.tolik.io", true);
        yield return new("https://www.tolik.io", true);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
