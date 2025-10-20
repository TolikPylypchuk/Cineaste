using System.Globalization;

using Microsoft.Extensions.Logging;

namespace Cineaste.Services.List;

public sealed class DefaultListCreator(ILogger<DefaultListCreator> logger) : IDefaultListCreator
{
    public CineasteList CreateDefaultList()
    {
        logger.LogDebug("Creating a default list");

        var config = new ListConfiguration(
            Id.Create<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            ListSortingConfiguration.CreateDefault());

        var list = new CineasteList(Id.Create<CineasteList>(), config);

        var black = new Color("#000000");
        var blue = new Color("#3949ab");
        var green = new Color("#43a047");
        var lightBlue = new Color("#1e88e5");
        var red = new Color("#e35953");
        var darkRed = new Color("#b71c1c");

        var liveActionMovie = new MovieKind(Id.Create<MovieKind>(), "Live-Action", black, red, darkRed);
        var animatedMovie = new MovieKind(Id.Create<MovieKind>(), "Animation", green, red, darkRed);
        var liveActionSeries = new SeriesKind(Id.Create<SeriesKind>(), "Live-Action", blue, red, darkRed);
        var animatedSeries = new SeriesKind(Id.Create<SeriesKind>(), "Animation", lightBlue, red, darkRed);

        list.AddMovieKind(liveActionMovie);
        list.AddMovieKind(animatedMovie);
        list.AddSeriesKind(liveActionSeries);
        list.AddSeriesKind(animatedSeries);

        return list;
    }
}
