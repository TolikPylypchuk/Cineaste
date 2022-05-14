namespace Cineaste.Core;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class Require
{
    private static readonly Regex ImdbIdRegex = new(@"tt\d+", RegexOptions.Compiled);

    public static T NotNull<T>(T? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value ?? throw new ArgumentNullException(paramName);

    public static string NotBlank(string? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value is not null
            ? !String.IsNullOrWhiteSpace(value) ? value : throw new ArgumentOutOfRangeException(paramName)
            : throw new ArgumentNullException(paramName);

    public static int Positive(int value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int Month(int value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value >= 1 && value <= 12 ? value : throw new ArgumentOutOfRangeException(paramName);

    [return: NotNullIfNotNull("value")]
    public static string? ImdbId(string? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value == null || ImdbIdRegex.IsMatch(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    [return: NotNullIfNotNull("value")]
    public static string? Url(string? value, [CallerArgumentExpression("value")] string? paramName = null) =>
        value == null || Uri.IsWellFormedUriString(value, UriKind.Absolute) && value.StartsWith("http")
            ? value
            : throw new ArgumentOutOfRangeException(paramName);
}
