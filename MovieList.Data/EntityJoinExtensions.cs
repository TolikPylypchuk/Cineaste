using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;

namespace MovieList.Data
{
    internal static class EntityJoinExtensions
    {
        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<Kind> kinds)
            => kinds
                .GroupJoin(
                    movies,
                    kind => kind.Id,
                    movie => movie.KindId,
                    (kind, moviesWithKind) =>
                    {
                        kind.Movies = moviesWithKind.ToList();

                        foreach (var movie in kind.Movies)
                        {
                            movie.Kind = kind;
                            movie.KindId = kind.Id;
                        }

                        return kind.Movies;
                    })
                .SelectMany(m => m);

        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<Title> titles)
            => movies.GroupJoin(
                titles.Where(title => title.MovieId != null),
                movie => movie.Id,
                title => title.MovieId,
                (movie, movieTitles) =>
                {
                    movie.Titles = movieTitles.ToList();

                    foreach (var title in movie.Titles)
                    {
                        title.Movie = movie;
                        title.MovieId = movie.Id;
                    }

                    return movie;
                });

        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<MovieSeriesEntry> entries)
            => movies.GroupJoin(
                entries.Where(entry => entry.MovieId != null),
                movie => movie.Id,
                entry => entry.MovieId,
                (movie, movieEntries) =>
                {
                    var entry = movieEntries.FirstOrDefault();

                    if (entry != null)
                    {
                        movie.Entry = entry;
                        entry.Movie = movie;
                        entry.MovieId = movie.Id;
                    }

                    return movie;
                });

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Kind> kinds)
            => kinds
                .GroupJoin(
                    seriesList,
                    kind => kind.Id,
                    series => series.KindId,
                    (kind, seriesWithKind) =>
                    {
                        kind.Series = seriesWithKind.ToList();

                        foreach (var series in kind.Series)
                        {
                            series.Kind = kind;
                            series.KindId = kind.Id;
                        }

                        return kind.Series;
                    })
                .SelectMany(s => s);

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Title> titles)
            => seriesList.GroupJoin(
                titles.Where(title => title.SeriesId != null),
                series => series.Id,
                title => title.SeriesId,
                (series, seriesTitles) =>
                {
                    series.Titles = seriesTitles.ToList();

                    foreach (var title in series.Titles)
                    {
                        title.Series = series;
                        title.SeriesId = series.Id;
                    }

                    return series;
                });

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Season> seasons)
            => seriesList.GroupJoin(
                seasons,
                series => series.Id,
                season => season.SeriesId,
                (series, seriesSeasons) =>
                {
                    series.Seasons = seriesSeasons.ToList();

                    foreach (var season in series.Seasons)
                    {
                        season.Series = series;
                        season.SeriesId = series.Id;
                    }

                    return series;
                });

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<SpecialEpisode> episodes)
            => seriesList.GroupJoin(
                episodes,
                series => series.Id,
                episode => episode.SeriesId,
                (series, seriesEpisodes) =>
                {
                    series.SpecialEpisodes = seriesEpisodes.ToList();

                    foreach (var episode in series.SpecialEpisodes)
                    {
                        episode.Series = series;
                        episode.SeriesId = series.Id;
                    }

                    return series;
                });

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<MovieSeriesEntry> entries)
            => seriesList.GroupJoin(
                entries.Where(entry => entry.SeriesId != null),
                movie => movie.Id,
                entry => entry.SeriesId,
                (series, seriesEntries) =>
                {
                    var entry = seriesEntries.FirstOrDefault();

                    if (entry != null)
                    {
                        series.Entry = entry;
                        entry.Series = series;
                        entry.SeriesId = series.Id;
                    }

                    return series;
                });

        public static IEnumerable<Season> Join(this IEnumerable<Season> seasons, IEnumerable<Period> periods)
            => seasons.GroupJoin(
                periods,
                season => season.Id,
                period => period.SeasonId,
                (season, seasonPeriods) =>
                {
                    season.Periods = seasonPeriods.ToList();

                    foreach (var period in season.Periods)
                    {
                        period.Season = season;
                        period.SeasonId = season.Id;
                    }

                    return season;
                });

        public static IEnumerable<Season> Join(this IEnumerable<Season> seasons, IEnumerable<Title> titles)
            => seasons.GroupJoin(
                titles.Where(title => title.SeasonId != null),
                season => season.Id,
                title => title.SeasonId,
                (season, seasonTitles) =>
                {
                    season.Titles = seasonTitles.ToList();

                    foreach (var title in season.Titles)
                    {
                        title.Season = season;
                        title.SeasonId = season.Id;
                    }

                    return season;
                });

        public static IEnumerable<SpecialEpisode> Join(this IEnumerable<SpecialEpisode> episodes, IEnumerable<Title> titles)
            => episodes.GroupJoin(
                titles.Where(title => title.SpecialEpisodeId != null),
                episode => episode.Id,
                title => title.SpecialEpisodeId,
                (episode, episodeTitles) =>
                {
                    episode.Titles = episodeTitles.ToList();

                    foreach (var title in episode.Titles)
                    {
                        title.SpecialEpisode = episode;
                        title.SpecialEpisodeId = episode.Id;
                    }

                    return episode;
                });

        public static IEnumerable<MovieSeries> Join(this IEnumerable<MovieSeries> movieSeriesList, IEnumerable<Title> titles)
            => movieSeriesList.GroupJoin(
                titles.Where(title => title.MovieSeriesId != null),
                movieSeries => movieSeries.Id,
                title => title.MovieSeriesId,
                (movieSeries, movieSeriesTitles) =>
                {
                    movieSeries.Titles = movieSeriesTitles.ToList();

                    foreach (var title in movieSeries.Titles)
                    {
                        title.MovieSeries = movieSeries;
                        title.MovieSeriesId = movieSeries.Id;
                    }

                    return movieSeries;
                });

        public static IEnumerable<MovieSeries> Join(this IEnumerable<MovieSeries> movieSeriesList, IList<MovieSeriesEntry> entries)
            => movieSeriesList
                .GroupJoin(
                    entries.Where(entry => entry.MovieSeriesId != null),
                    movieSeries => movieSeries.Id,
                    entry => entry.MovieSeriesId,
                    (movieSeries, movieSeriesEntries) =>
                    {
                        var entry = movieSeriesEntries.FirstOrDefault();

                        if (entry != null)
                        {
                            movieSeries.Entry = entry;
                            entry.MovieSeries = movieSeries;
                            entry.MovieSeriesId = movieSeries.Id;
                        }

                        return movieSeries;
                    })
                .GroupJoin(
                    entries,
                    movieSeries => movieSeries.Id,
                    entry => entry.ParentSeriesId,
                    (movieSeries, entriesOfMovieSeries) =>
                    {
                        movieSeries.Entries = entriesOfMovieSeries.ToList();

                        foreach (var entry in movieSeries.Entries)
                        {
                            entry.ParentSeries = movieSeries;
                            entry.ParentSeriesId = movieSeries.Id;
                        }

                        return movieSeries;
                    });
    }
}
