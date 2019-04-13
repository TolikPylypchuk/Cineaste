using System.Linq;

namespace MovieList
{
    public static class Util
    {
        public static int GetHashCode(params object[] properties)
            => properties.Select(p => p.GetHashCode()).Aggregate(17, (acc, hash) => unchecked(acc * 23 + hash));
    }
}
