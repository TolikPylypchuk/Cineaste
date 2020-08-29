using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("MovieTags")]
    public sealed class MovieTag
    {
        [ExplicitKey]
        public int MovieId { get; set; }

        [ExplicitKey]
        public int TagId { get; set; }
    }
}
