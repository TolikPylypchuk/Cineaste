using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Series))]
    public class Series : EntityBase
    {
        public bool IsWatched { get; set; }
        public bool IsMiniseries { get; set; }

        public SeriesStatus Status { get; set; }

        [Url]
        [StringLength(256)]
        public string? ImdbLink { get; set; }

        [Url]
        [StringLength(256)]
        public string? PosterUrl { get; set; }

        public int KindId { get; set; }

        [ForeignKey(nameof(KindId))]
        public virtual Kind Kind { get; set; }

        public virtual MovieSeriesEntry? Entry { get; set; }

        public virtual IList<Title> Titles { get; set; } = new List<Title>();

        public virtual IList<Season> Seasons { get; set; } = new List<Season>();

        public virtual IList<SpecialEpisode> SpecialEpisodes { get; set; } = new List<SpecialEpisode>();

        [NotMapped]
        public Title Title
            => this.Titles
                .Where(title => !title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        [NotMapped]
        public Title OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderBy(title => title.Priority)
                .First();

        [NotMapped]
        public int StartYear
            => this.Seasons
                .SelectMany(season => season.Periods)
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .First()
                .StartYear;

        [NotMapped]
        public int EndYear
            => this.Seasons
                .SelectMany(season => season.Periods)
                .OrderByDescending(period => period.EndYear)
                .ThenByDescending(period => period.EndMonth)
                .First()
                .EndYear;

        public override string ToString()
            => $"Series: {this.Id}";
    }
}
