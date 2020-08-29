using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("FranchiseTags")]
    public sealed class FranchiseTag
    {
        [ExplicitKey]
        public int FranchiseId { get; set; }

        [ExplicitKey]
        public int TagId { get; set; }
    }
}
