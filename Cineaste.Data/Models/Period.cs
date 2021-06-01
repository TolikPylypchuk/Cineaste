using Dapper.Contrib.Extensions;

namespace Cineaste.Data.Models
{
    [Table("Periods")]
    public sealed class Period : EntityBase
    {
        public int StartMonth { get; set; }
        public int StartYear { get; set; }

        public int EndMonth { get; set; }
        public int EndYear { get; set; }

        public bool IsSingleDayRelease { get; set; }

        public int NumberOfEpisodes { get; set; }

        public string? RottenTomatoesLink { get; set; }

        public string? PosterUrl { get; set; }

        public int SeasonId { get; set; }

        [Write(false)]
        public Season Season { get; set; } = null!;

        public override string ToString()
        {
            string content = this.IsSingleDayRelease
                ? $"{this.StartMonth}.{this.StartYear}"
                : this.StartYear == this.EndYear
                    ? this.StartMonth == this.EndMonth
                        ? $"{this.StartMonth}.{this.StartYear}"
                        : $"{this.StartMonth}-{this.EndMonth}.{this.StartYear}"
                    : $"{this.StartMonth}.{this.StartYear}-{this.EndMonth}.{this.EndYear}";

            return $"Period #{this.Id}: {content}";
        }
    }
}
