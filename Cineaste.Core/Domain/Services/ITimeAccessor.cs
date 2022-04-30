namespace Cineaste.Core.Domain.Services;

public interface ITimeAccessor
{
    DateTimeOffset Now { get; }
}
