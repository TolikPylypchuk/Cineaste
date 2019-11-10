using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Settings")]
    internal sealed class Setting : EntityBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
