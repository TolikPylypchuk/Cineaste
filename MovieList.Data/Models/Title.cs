using System;
using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Titles")]
    public sealed class Title : EntityBase
    {
        public string Name { get; set; } = String.Empty;

        public int Priority { get; set; } = 1;

        public bool IsOriginal { get; set; }

        public int? MovieId { get; set; }

        [Write(false)]
        public Movie? Movie { get; set; }

        public int? SeriesId { get; set; }

        [Write(false)]
        public Series? Series { get; set; }

        public int? SeasonId { get; set; }

        [Write(false)]
        public Season? Season { get; set; }

        public int? SpecialEpisodeId { get; set; }

        [Write(false)]
        public SpecialEpisode? SpecialEpisode { get; set; }

        public int? MovieSeriesId { get; set; }

        [Write(false)]
        public MovieSeries? MovieSeries { get; set; }

        public static string ToString(IEnumerable<Title> titles)
        {
            var titlesAsList = titles.ToList();

            var sortedTitles = titlesAsList
                .Where(t => !t.IsOriginal)
                .OrderBy(t => t.Priority)
                .Select(t => t.Name)
                .Union(titlesAsList
                    .Where(t => t.IsOriginal)
                    .OrderBy(t => t.Priority)
                    .Select(t => t.Name))
                .ToList();

            return sortedTitles.Count == 0
                ? String.Empty
                : sortedTitles.Aggregate((acc, item) => $"{acc}/{item}");
        }

        public override string ToString()
            => $"Title #{this.Id}: {this.Name}";
    }
}
