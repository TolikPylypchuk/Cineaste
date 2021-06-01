using Dapper.Contrib.Extensions;

namespace Cineaste.Data.Models
{
    [Table("TagImplications")]
    internal sealed class TagImplication : EntityBase
    {
        public int PremiseId { get; set; }
        public int ConsequenceId { get; set; }
    }
}
