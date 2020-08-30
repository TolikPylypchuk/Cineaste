using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("FranchiseTags")]
    internal sealed class FranchiseTag : EntityBase
    {
        public int FranchiseId { get; set; }
        public int TagId { get; set; }
    }
}
