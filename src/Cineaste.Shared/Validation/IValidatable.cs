namespace Cineaste.Shared.Validation;

public interface IValidatable<T>
    where T : IValidatable<T>
{
    static abstract IValidator<T> Validator { get; }
}
