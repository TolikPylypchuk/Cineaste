using System;
using System.Collections.Generic;

namespace MovieList.Core.Comparers
{
    public sealed class PropertyComparer<T, TProperty> : NullableComparerBase<T>
    {
        private readonly Func<T, TProperty> propertyGetter;
        private readonly IComparer<TProperty> propertyComparer;

        public PropertyComparer(
            Func<T, TProperty> propertyGetter,
            IComparer<TProperty> propertyComparer,
            NullComparison nullComparison = NullComparison.NullsFirst)
            : base(nullComparison)
        {
            this.propertyGetter = propertyGetter;
            this.propertyComparer = propertyComparer;
        }

        protected override int CompareSafe(T x, T y)
            => this.propertyComparer.Compare(this.propertyGetter(x), this.propertyGetter(y));
    }
}
