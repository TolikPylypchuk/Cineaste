using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Settings")]
    internal sealed class Setting : EntityBase
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
