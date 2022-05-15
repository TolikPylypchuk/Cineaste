namespace Cineaste.Core.Domain;

public record struct Id<T>(Guid Value);

public static class Id
{
    public static Id<T> Create<T>() =>
        new(Guid.NewGuid());
}