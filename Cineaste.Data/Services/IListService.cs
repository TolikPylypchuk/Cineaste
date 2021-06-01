using System.Collections.Generic;

using Cineaste.Data.Models;

namespace Cineaste.Data.Services
{
    public interface IListService
    {
        EntireList GetList(IEnumerable<Kind> kinds, IEnumerable<Tag> tags);
    }
}
