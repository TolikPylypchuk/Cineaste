using System;
using System.Collections.Generic;

namespace MovieList.Comparers
{
    public sealed class PropertyComparer<T, TProperty> : IComparer<T>
    {
        private readonly Func<T, TProperty> propertyGetter;
        private readonly IComparer<TProperty> propertyComparer;

        public PropertyComparer(Func<T, TProperty> propertyGetter, IComparer<TProperty> propertyComparer)
        {
            this.propertyGetter = propertyGetter;
            this.propertyComparer = propertyComparer;
        }

        public int Compare(T x, T y)
            => this.propertyComparer.Compare(this.propertyGetter(x), this.propertyGetter(y));
    }
}
