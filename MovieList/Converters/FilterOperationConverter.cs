using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class FilterOperationConverter : IBindingTypeConverter
    {
        private readonly Dictionary<FilterOperation, string> filterTypeToString;
        private readonly Dictionary<string, FilterOperation> stringToFilterType;

        public FilterOperationConverter()
        {
            this.filterTypeToString = new Dictionary<FilterOperation, string>
            {
                [FilterOperation.None] = String.Empty,
                [FilterOperation.Is] = Messages.FilterOperationIs,
                [FilterOperation.StartsWith] = Messages.FilterOperationStartsWith,
                [FilterOperation.EndsWith] = Messages.FilterOperationEndsWith,
                [FilterOperation.LessThan] = Messages.FilterOperationLessThan,
                [FilterOperation.GreaterThan] = Messages.FilterOperationGreaterThan,
                [FilterOperation.Between] = Messages.FilterOperationBetween,
                [FilterOperation.Include] = Messages.FilterOperationInclude,
                [FilterOperation.Exclude] = Messages.FilterOperationExclude,
                [FilterOperation.HaveCategory] = Messages.FilterOperationHaveCategory,
                [FilterOperation.NoneOfCategory] = Messages.FilterOperationNoneOfCategory
            };

            this.stringToFilterType = filterTypeToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(FilterOperation) || toType == typeof(FilterOperation)
                ? 10000
                : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case FilterOperation filterOperation:
                    result = this.filterTypeToString[filterOperation];
                    return true;
                case string str:
                    result = this.stringToFilterType[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
