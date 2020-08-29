using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("SeriesTags")]
    public sealed class SeriesTag
    {
        [ExplicitKey]
        public int SeriesId { get; set; }

        [ExplicitKey]
        public int TagId { get; set; }
    }
}
