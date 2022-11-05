namespace Cineaste;

using System.Collections;

internal abstract class TestDataBase : IEnumerable<object?[]>
{
    private List<object?[]> Data { get; } = new();

    public IEnumerator<object?[]> GetEnumerator()
    {
        foreach (var data in this.Data)
        {
            yield return data;
        }
    }

    protected void Add(params object?[] data) =>
        this.Data.Add(data);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
