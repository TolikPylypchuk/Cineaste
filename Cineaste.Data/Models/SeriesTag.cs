using Dapper.Contrib.Extensions;

namespace Cineaste.Data.Models
{
    [Table("SeriesTags")]
    internal sealed class SeriesTag : EntityBase
    {
        public int SeriesId { get; set; }
        public int TagId { get; set; }
    }
}
