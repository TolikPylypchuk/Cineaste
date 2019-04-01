using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public class IdEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : EntityBase
    {
        private IdEqualityComparer() { }

        static IdEqualityComparer()
            => Instance = new IdEqualityComparer<TEntity>();

        public static IEqualityComparer<TEntity> Instance { get; }

        public bool Equals(TEntity x, TEntity y)
            => x == null
                ? y == null ? true : false
                : x.Id.Equals(y.Id);

        public int GetHashCode(TEntity obj)
            => obj == null ? 1 : obj.Id.GetHashCode();
    }
}
