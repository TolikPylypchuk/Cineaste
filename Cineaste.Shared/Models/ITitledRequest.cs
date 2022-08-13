namespace Cineaste.Shared.Models;

public interface ITitledRequest
{
    public ImmutableList<TitleRequest> Titles { get; }

    public ImmutableList<TitleRequest> OriginalTitles { get; }
}
