using System.Runtime.CompilerServices;

namespace Cineaste.Core;

public static partial class Require
{
    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value ?? throw new ArgumentNullException(paramName);

    public static string NotEmpty(string value,[CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        !String.IsNullOrEmpty(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    public static string NotBlank(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        !String.IsNullOrWhiteSpace(value) ? value : throw new ArgumentOutOfRangeException(paramName);

    public static C NotEmpty<C, T>(C value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where C : IReadOnlyCollection<T> =>
        value.Count != 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static T[] NotEmpty<T>(T[] value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value.Length != 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int Positive(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static int? Positive(int? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) =>
        value is null || value > 0 ? value : throw new ArgumentOutOfRangeException(paramName);

    public static TEnum ValidEnum<TEnum>(
        TEnum value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TEnum : struct, Enum =>
        Enum.IsDefined(value) ? value : throw new ArgumentOutOfRangeException(paramName);
}
