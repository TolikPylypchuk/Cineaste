
using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class KindService : SettingsEntityServiceBase<Kind>
    {
        public KindService(string file)
            : base(file)
        { }

        protected override string GetAllMessage
            => "Getting all kinds";

        protected override string UpdateAllMessage
            => "Updating all kinds";

        protected override string DeleteExceptionMessage
            => "Cannot delete kinds that have movies or series attached to them";

        protected override bool CanDelete(Kind kind)
            => kind.Movies.Count == 0 && kind.Series.Count == 0;
    }
}
