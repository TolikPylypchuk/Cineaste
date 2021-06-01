using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;

namespace MovieList.Data
{
    internal static class EntityJoinExtensions
    {
        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<Kind> kinds) =>
            kinds
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

        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<Title> titles) =>
            movies.GroupJoin(
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

        public static IEnumerable<Movie> Join(this IEnumerable<Movie> movies, IEnumerable<FranchiseEntry> entries) =>
            movies.GroupJoin(
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

        public static IEnumerable<Movie> Join(
            this IEnumerable<Movie> movies,
            IEnumerable<MovieTag> movieTags,
            IDictionary<int, Tag> tagsById) =>
            movies.GroupJoin(
                movieTags,
                movie => movie.Id,
                movieTag => movieTag.MovieId,
                (movie, movieTags) =>
                {
                    movie.Tags = movieTags
                        .Select(movieTag => tagsById[movieTag.TagId])
                        .Do(tag => tag.Movies.Add(movie))
                        .ToHashSet();

                    return movie;
                });

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Kind> kinds) =>
            kinds
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

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Title> titles) =>
            seriesList.GroupJoin(
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

        public static IEnumerable<Series> Join(this IEnumerable<Series> seriesList, IEnumerable<Season> seasons) =>
            seriesList.GroupJoin(
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

        public static IEnumerable<Series> Join(
            this IEnumerable<Series> seriesList,
            IEnumerable<SpecialEpisode> episodes) =>
            seriesList.GroupJoin(
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

        public static IEnumerable<Series> Join(
            this IEnumerable<Series> seriesList,
            IEnumerable<FranchiseEntry> entries) =>
            seriesList.GroupJoin(
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

        public static IEnumerable<Season> Join(this IEnumerable<Season> seasons, IEnumerable<Period> periods) =>
            seasons.GroupJoin(
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

        public static IEnumerable<Season> Join(this IEnumerable<Season> seasons, IEnumerable<Title> titles) =>
            seasons.GroupJoin(
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

        public static IEnumerable<SpecialEpisode> Join(
            this IEnumerable<SpecialEpisode> episodes,
            IEnumerable<Title> titles) =>
            episodes.GroupJoin(
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

        public static IEnumerable<Series> Join(
            this IEnumerable<Series> seriesList,
            IEnumerable<SeriesTag> seriesTags,
            IDictionary<int, Tag> tagsById) =>
            seriesList.GroupJoin(
                seriesTags,
                series => series.Id,
                seriesTag => seriesTag.SeriesId,
                (series, seriesTags) =>
                {
                    series.Tags = seriesTags
                        .Select(seriesTag => tagsById[seriesTag.TagId])
                        .Do(tag => tag.Series.Add(series))
                        .ToHashSet();

                    return series;
                });

        public static IEnumerable<Franchise> Join(this IEnumerable<Franchise> franchises, IEnumerable<Title> titles) =>
            franchises.GroupJoin(
                titles.Where(title => title.FranchiseId != null),
                franchise => franchise.Id,
                title => title.FranchiseId,
                (franchise, franchiseTitles) =>
                {
                    franchise.Titles = franchiseTitles.ToList();

                    foreach (var title in franchise.Titles)
                    {
                        title.Franchise = franchise;
                        title.FranchiseId = franchise.Id;
                    }

                    return franchise;
                });

        public static IEnumerable<Franchise> Join(
            this IEnumerable<Franchise> franchises,
            IList<FranchiseEntry> entries) =>
            franchises
                .GroupJoin(
                    entries.Where(entry => entry.FranchiseId != null),
                    franchise => franchise.Id,
                    entry => entry.FranchiseId,
                    (franchise, franchiseEntries) =>
                    {
                        var entry = franchiseEntries.FirstOrDefault();

                        if (entry != null)
                        {
                            franchise.Entry = entry;
                            entry.Franchise = franchise;
                            entry.FranchiseId = franchise.Id;
                        }

                        return franchise;
                    })
                .GroupJoin(
                    entries,
                    franchise => franchise.Id,
                    entry => entry.ParentFranchiseId,
                    (franchise, franchiseEntries) =>
                    {
                        franchise.Entries = franchiseEntries.ToList();

                        foreach (var entry in franchise.Entries)
                        {
                            entry.ParentFranchise = franchise;
                            entry.ParentFranchiseId = franchise.Id;
                        }

                        return franchise;
                    });
    }
}
