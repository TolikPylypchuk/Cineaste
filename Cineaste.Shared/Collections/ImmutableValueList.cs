// Based on this GitHub Gist: https://gist.github.com/jhgbrt/4bf2cf7e5c077f7326c8b82160a9c59a

namespace Cineaste.Shared.Collections;

using System.Collections;
using System.Collections.Immutable;

public sealed class ImmutableValueList<T> : IImmutableList<T>, IEquatable<IImmutableList<T>>
{
    private readonly IImmutableList<T> list;

    public ImmutableValueList(IImmutableList<T> list) =>
        this.list = list;

    public T this[int index] => list[index];

    public int Count => list.Count;

    public IImmutableList<T> Add(T value) =>
        list.Add(value).AsValue();

    public IImmutableList<T> AddRange(IEnumerable<T> items) =>
        list.AddRange(items).AsValue();

    public IImmutableList<T> Clear() =>
        list.Clear().AsValue();

    public IEnumerator<T> GetEnumerator() =>
        list.GetEnumerator();

    public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer = null) =>
        list.IndexOf(item, index, count, equalityComparer);

    public IImmutableList<T> Insert(int index, T element) =>
        list.Insert(index, element).AsValue();

    public IImmutableList<T> InsertRange(int index, IEnumerable<T> items) =>
        list.InsertRange(index, items).AsValue();

    public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer = null) =>
        list.LastIndexOf(item, index, count, equalityComparer);

    public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer = null) =>
        list.Remove(value, equalityComparer).AsValue();

    public IImmutableList<T> RemoveAll(Predicate<T> match) =>
        list.RemoveAll(match).AsValue();

    public IImmutableList<T> RemoveAt(int index) =>
        list.RemoveAt(index).AsValue();

    public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer = null) =>
        list.RemoveRange(items, equalityComparer).AsValue();

    public IImmutableList<T> RemoveRange(int index, int count) =>
        list.RemoveRange(index, count).AsValue();

    public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer = null) =>
        list.Replace(oldValue, newValue, equalityComparer).AsValue();

    public IImmutableList<T> SetItem(int index, T value) =>
        list.SetItem(index, value);

    public override bool Equals(object? obj) =>
        obj is IImmutableList<T> list && this.Equals(list);

    public bool Equals(IImmutableList<T>? other) =>
        this.SequenceEqual(other ?? ImmutableList<T>.Empty);

    public override int GetHashCode()
    {
        unchecked
        {
            return this.Aggregate(19, (h, i) => h * 19 + i?.GetHashCode() ?? 0);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        list.GetEnumerator();
}

public static class ImmutableValueList
{
    public static ImmutableValueList<T> AsValue<T>(this IImmutableList<T> list) =>
        new(list);
}
