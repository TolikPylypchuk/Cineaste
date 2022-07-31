namespace Cineaste.Shared.Models;

public interface ITitledModel
{
    public ImmutableList<TitleModel> Titles { get; }

    public ImmutableList<TitleModel> OriginalTitles { get; }
}
