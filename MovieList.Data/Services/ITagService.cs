using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetAllTags();
    }
}
