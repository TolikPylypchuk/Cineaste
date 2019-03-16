using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MovieList.Data.Properties;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data.Models
{
    [Table(nameof(MovieContext.Periods))]
    public class Period : EntityBase
    {
        [Range(1, 12, ErrorMessageResourceName = "InvalidMonth", ErrorMessageResourceType = typeof(Messages))]
        public int StartMonth { get; set; }

        [Range(1950, 2100, ErrorMessageResourceName = "TV.InvalidYear", ErrorMessageResourceType = typeof(Messages))]
        public int StartYear { get; set; }

        [Range(1, 12, ErrorMessageResourceName = "InvalidMonth", ErrorMessageResourceType = typeof(Messages))]
        public int EndMonth { get; set; }

        [Range(1950, 2100, ErrorMessageResourceName = "TV.InvalidYear", ErrorMessageResourceType = typeof(Messages))]
        public int EndYear { get; set; }

        public bool IsSingleDayRelease { get; set; }

        [Range(1, 50, ErrorMessageResourceName = "Period.InvalidNumberOfEpisodes", ErrorMessageResourceType = typeof(Messages))]
        public int NumberOfEpisodes { get; set; }

        public int SeasonId { get; set; }

        [ForeignKey(nameof(SeasonId))]
        public Season Season { get; set; }
    }
}

#pragma warning enable CS8618 // Non-nullable field is uninitialized.
