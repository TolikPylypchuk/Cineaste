using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("TagImplications")]
    public sealed class TagImplication : EntityBase
    {
        [ExplicitKey]
        public int PremiseId { get; set; }

        [ExplicitKey]
        public int ConsequenceId { get; set; }
    }
}
