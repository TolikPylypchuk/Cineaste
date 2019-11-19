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
                        kind.Movies.ForEach(movie => movie.Kind = kind);
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
                    movie.Titles.ForEach(title => title.Movie = movie);
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
                        kind.Series.ForEach(movie => movie.Kind = kind);
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
                    series.Titles.ForEach(title => title.Series = series);
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
                    series.Seasons.ForEach(season => season.Series = series);
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
                    series.SpecialEpisodes.ForEach(episode => episode.Series = series);
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
                    season.Periods.ForEach(period => period.Season = season);
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
                    season.Titles.ForEach(title => title.Season = season);
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
                    episode.Titles.ForEach(title => title.SpecialEpisode = episode);
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
                    movieSeries.Titles.ForEach(title => title.MovieSeries = movieSeries);
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
                        movieSeries.Entries.ForEach(entry => entry.ParentSeries = movieSeries);
                        return movieSeries;
                    });
    }
}
