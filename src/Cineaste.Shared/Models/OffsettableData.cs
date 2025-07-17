namespace Cineaste.Shared.Models;

public sealed record OffsettableData<T>(ImmutableList<T> Items, OffsetMetadata Metadata);

public sealed record OffsetMetadata(int Offset, int Size, int TotalItems);
