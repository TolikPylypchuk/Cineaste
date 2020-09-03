using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IListService
    {
        WholeList GetList(IEnumerable<Kind> kinds, IEnumerable<Tag> tags);
    }
}
