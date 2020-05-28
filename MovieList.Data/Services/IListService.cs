using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IListService
    {
        MovieList GetList(IList<Kind> kinds);
    }
}
