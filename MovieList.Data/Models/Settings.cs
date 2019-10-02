using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Settings")]
    public sealed class Settings : EntityBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
