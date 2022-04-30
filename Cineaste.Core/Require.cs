namespace Cineaste.Core;

using System.Runtime.CompilerServices;

public static class Require
{
    public static T NotNull<T>(T? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value ?? throw new ArgumentNullException(paramName);

    public static string NotBlank(string? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value is not null
            ? !String.IsNullOrWhiteSpace(value) ? value : throw new ArgumentOutOfRangeException(paramName)
            : throw new ArgumentNullException(paramName);

    public static int Positive(int value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);
}
