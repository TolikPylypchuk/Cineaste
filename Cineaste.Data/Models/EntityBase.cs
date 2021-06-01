using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
