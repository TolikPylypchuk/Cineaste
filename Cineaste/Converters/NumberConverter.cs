using System;

using ReactiveUI;

namespace Cineaste.Converters
{
    public sealed class NumberConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType) =>
            fromType == typeof(int) && toType == typeof(double) ||
            fromType == typeof(double) && toType == typeof(int)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case int value:
                    result = (double)value;
                    return true;
                case double value when !Double.IsNaN(value) && !Double.IsInfinity(value):
                    result = (int)value;
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
