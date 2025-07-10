namespace Cineaste.Shared.Models;

public interface ITitledRequest
{
    public ImmutableValueList<TitleRequest> Titles { get; }

    public ImmutableValueList<TitleRequest> OriginalTitles { get; }
}
