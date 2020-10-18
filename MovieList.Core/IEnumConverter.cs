using System;

namespace MovieList.Core
{
    public interface IEnumConverter<TEnum>
        where TEnum : Enum
    {
        string ToString(TEnum value);
        TEnum FromString(string str);
    }
}
