using System.Collections;

namespace Cineaste.Client.FormModels;

public class TitleList : IList<string>
{
    private readonly List<string> titles = new() { String.Empty };
    private readonly Action changeCallback;

    public TitleList(Action changeCallback) =>
        this.changeCallback = changeCallback;

    public int Count =>
        this.titles.Count;

    public bool IsReadOnly =>
        false;

    public string this[int index]
    {
        get => this.titles[index];
        set
        {
            this.titles[index] = value;
            this.changeCallback();
        }
    }

    public IEnumerator<string> GetEnumerator() =>
        this.titles.GetEnumerator();

    public int IndexOf(string item) =>
        this.titles.IndexOf(item);

    public void Add(string item)
    {
        this.titles.Add(item);
        this.changeCallback();
    }

    public void Insert(int index, string item)
    {
        this.titles.Insert(index, item);
        this.changeCallback();
    }

    public bool Remove(string item)
    {
        bool result = this.titles.Remove(item);
        this.changeCallback();

        return result;
    }

    public void RemoveAt(int index)
    {
        this.titles.RemoveAt(index);
        this.changeCallback();
    }

    public void Clear()
    {
        this.titles.Clear();
        this.changeCallback();
    }

    public bool Contains(string item) =>
        this.titles.Contains(item);

    public void CopyTo(string[] array, int arrayIndex) =>
        this.titles.CopyTo(array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
