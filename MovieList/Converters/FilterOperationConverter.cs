using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Core;
using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Converters
{
    public sealed class FilterOperationConverter : IBindingTypeConverter, IEnumConverter<FilterOperation>
    {
        private readonly Dictionary<FilterOperation, string> filterOperationToString;
        private readonly Dictionary<string, FilterOperation> stringToFilterOperation;

        public FilterOperationConverter()
        {
            this.filterOperationToString = new Dictionary<FilterOperation, string>
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
                [FilterOperation.HaveCategory] = Messages.FilterOperationHaveCategory
            };

            this.stringToFilterOperation = filterOperationToString.ToDictionary(e => e.Value, e => e.Key);
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
                    result = this.filterOperationToString[filterOperation];
                    return true;
                case string str:
                    result = this.stringToFilterOperation[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }

        public string ToString(FilterOperation filterOperation)
            => this.filterOperationToString[filterOperation];

        public FilterOperation FromString(string str)
            => this.stringToFilterOperation.ContainsKey(str)
                ? this.stringToFilterOperation[str]
                : throw new ArgumentOutOfRangeException(nameof(str));
    }
}
