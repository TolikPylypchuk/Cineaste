using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IKindService
    {
        IEnumerable<Kind> GetAllKinds();
        void UpdateKinds(IEnumerable<Kind> kinds);
    }
}
