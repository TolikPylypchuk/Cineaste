using System;
using System.Collections.Generic;
using System.Linq;

using Cineaste.Core;

using ReactiveUI;

namespace Cineaste.Converters
{
    public abstract class EnumConverter<TEnum> : IBindingTypeConverter, IEnumConverter<TEnum>
        where TEnum : Enum
    {
        private readonly Dictionary<TEnum, string> enumToString;
        private readonly Dictionary<string, TEnum> stringToEnum;

        public EnumConverter()
        {
            this.enumToString = this.CreateConverterDictionary();
            this.stringToEnum = this.enumToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType) =>
            fromType == typeof(TEnum) || toType == typeof(TEnum)
                ? 10000
                : fromType == typeof(TEnum) || toType == typeof(TEnum)
                    ? 1000
                    : 0;

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            switch (from)
            {
                case TEnum value:
                    result = this.enumToString[value];
                    return true;
                case int value:
                    result = this.enumToString[(TEnum)(object)value];
                    return true;
                case string str when toType == typeof(TEnum):
                    result = this.stringToEnum[str];
                    return true;
                case string str when toType == typeof(int):
                    result = (int)(object)this.stringToEnum[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }

        public string ToString(TEnum value) =>
            this.enumToString[value];

        public TEnum FromString(string str) =>
            this.stringToEnum.ContainsKey(str)
                ? this.stringToEnum[str]
                : throw new ArgumentOutOfRangeException(nameof(str));

        protected abstract Dictionary<TEnum, string> CreateConverterDictionary();
    }
}
