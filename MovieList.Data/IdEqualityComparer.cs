using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public class IdEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : EntityBase
    {
        private IdEqualityComparer()
        { }

        public static IdEqualityComparer<TEntity> Instance { get; } = new IdEqualityComparer<TEntity>();

#pragma warning disable 8509

        public bool Equals([AllowNull] TEntity x, [AllowNull] TEntity y)
            => (x, y) switch
            {
                (null, null) => true,
                (_, null) => false,
                (null, _) => false,
                var (left, right) => left.Id == right.Id
            };

#pragma warning restore 8509

        public int GetHashCode([AllowNull] TEntity obj)
            => obj?.Id.GetHashCode() ?? 1;
    }
}
