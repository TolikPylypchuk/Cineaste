using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using MovieList.Data.Models;

namespace MovieList.Data
{
    internal static class CompositeIdEqualityComparer
    {
        public static CompositeIdEqualityComparer<TagImplication> TagImplication { get; } =
            new(tagImplication => tagImplication.PremiseId, tagImplication => tagImplication.ConsequenceId);

        public static CompositeIdEqualityComparer<MovieTag> MovieTag { get; } =
            new(movieTag => movieTag.TagId, movieTag => movieTag.MovieId);

        public static CompositeIdEqualityComparer<SeriesTag> SeriesTag { get; } =
            new(seriesTag => seriesTag.TagId, seriesTag => seriesTag.SeriesId);

        public static CompositeIdEqualityComparer<FranchiseTag> FranchiseTag { get; } =
            new(franchiseTag => franchiseTag.TagId, franchiseTag => franchiseTag.FranchiseId);
    }

    internal sealed class CompositeIdEqualityComparer<TTag> : EqualityComparer<TTag>
        where TTag : EntityBase
    {
        private readonly Func<TTag, int>[] idSelectors;

        public CompositeIdEqualityComparer(params Func<TTag, int>[] idSelectors)
            => this.idSelectors = idSelectors.Length != 0
                ? idSelectors
                : throw new ArgumentOutOfRangeException(
                    nameof(idSelectors), "Cannot create an equality comparer with no ID selectors");

        public override bool Equals([AllowNull] TTag left, [AllowNull] TTag right)
            => (left, right) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                var (a, b) => this.idSelectors.All(idSelector => idSelector(a) == idSelector(b))
            };

        public override int GetHashCode([DisallowNull] TTag tag)
            => this.idSelectors.Select(selector => selector(tag)).Aggregate(HashCode.Combine);
    }
}
