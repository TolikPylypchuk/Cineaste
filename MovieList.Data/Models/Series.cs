using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using MovieList.Data.Properties;

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Series))]
    public class Series : EntityBase
    {
        public bool IsWatched { get; set; }

        [Url(ErrorMessageResourceName = "ImdbLinkInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "ImdbLinkTooLong", ErrorMessageResourceType = typeof(Messages))]
        public string? ImdbLink { get; set; }

        [Url(ErrorMessageResourceName = "Movie.PosterUrlInvalid", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(256, ErrorMessageResourceName = "Movie.PosterUrlTooLong", ErrorMessageResourceType = typeof(Messages))]
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
                .OrderByDescending(title => title.Priority)
                .First();

        [NotMapped]
        public Title OriginalTitle
            => this.Titles
                .Where(title => title.IsOriginal)
                .OrderByDescending(title => title.Priority)
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
