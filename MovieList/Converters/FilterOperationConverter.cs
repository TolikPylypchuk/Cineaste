using System;
using System.Collections.Generic;

using Cineaste.Core.ViewModels.Filters;
using Cineaste.Properties;

namespace Cineaste.Converters
{
    public sealed class FilterOperationConverter : EnumConverter<FilterOperation>
    {
        protected override Dictionary<FilterOperation, string> CreateConverterDictionary() =>
            new()
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
    }
}
