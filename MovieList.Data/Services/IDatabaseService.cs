using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data.Services
{
    public interface IDatabaseService
    {
        void CreateDatabase(Settings settings, IEnumerable<Kind> initialKinds);
        bool ValidateDatabase();
    }
}
