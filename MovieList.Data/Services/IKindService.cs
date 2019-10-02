using System.Collections.Generic;
using System.Threading.Tasks;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IKindService
    {
        Task<IEnumerable<Kind>> GetAllKindsAsync();
    }
}
