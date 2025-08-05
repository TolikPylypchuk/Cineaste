using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cineaste.Core;

public static partial class Require
{
    private static readonly Regex Sha256Regex = GenerateSha256Regex();

    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value ?? throw new ArgumentNullException(paramName);

    public static string NotBlank(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is not null
            ? !String.IsNullOrWhiteSpace(value) ? value : throw new ArgumentOutOfRangeException(paramName)
            : throw new ArgumentNullException(paramName);

    public static C NotEmpty<C, T>(this C value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where C : IReadOnlyCollection<T> =>
        value.Count != 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static T[] NotEmpty<T>(this T[] value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value.Length != 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static string NotEmpty(
        this string value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value.Length != 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int Positive(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int? Positive(int? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is null || value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int Month(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value >= 1 && value <= 12 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static TEnum ValidEnum<TEnum>(
        this TEnum value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TEnum : struct, Enum =>
        Enum.IsDefined(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    [return: NotNullIfNotNull(nameof(value))]
    public static string? ImdbId(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is null || ImdbIdRegex.IsMatch(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    [return: NotNullIfNotNull(nameof(value))]
    public static string? RottenTomatoesId(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is null || RottenTomatoesIdRegex.IsMatch(value)
            ? value
            : throw new ArgumentOutOfRangeException(paramName);

    public static string? Sha256(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is null || Sha256Regex.IsMatch(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    [GeneratedRegex("^[0-9a-f]{64}$", RegexOptions.Compiled)]
    private static partial Regex GenerateSha256Regex();
}
