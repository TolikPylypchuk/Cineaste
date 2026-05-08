using System.Diagnostics.CodeAnalysis;

namespace Cineaste.Common;

public static class Match
{
    [DoesNotReturn]
    public static T ImpossibleType<T>(object obj) =>
        throw new IncompleteMatchException($"Type {obj.GetType()} is not handled in a match expression");

    [DoesNotReturn]
    public static void ImpossibleType(object obj) =>
        throw new IncompleteMatchException($"Type {obj.GetType()} is not handled in a match expression");
}
