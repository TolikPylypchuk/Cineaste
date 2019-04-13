using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public class IdEqualityComparer<TEntity> : EqualityComparer<TEntity> where TEntity : EntityBase
    {
        private IdEqualityComparer() { }

        static IdEqualityComparer()
            => Instance = new IdEqualityComparer<TEntity>();

        public static IEqualityComparer<TEntity> Instance { get; }

        public override bool Equals(TEntity x, TEntity y)
            => x == null
                ? y == null ? true : false
                : x.Id.Equals(y.Id);

        public override int GetHashCode(TEntity e)
            => e == null ? 1 : e.Id.GetHashCode();
    }
}
