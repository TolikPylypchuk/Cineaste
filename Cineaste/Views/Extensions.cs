using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Media.Imaging;

using Cineaste.Core;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Views
{
    public static class Extensions
    {
        public static void SetEnumValues<TEnum>(this ComboBox comboBox, params TEnum[] values)
            where TEnum : struct, Enum
        {
            var converter = GetDefaultService<IEnumConverter<TEnum>>();

            comboBox.Items = (values.Length != 0 ? values : Enum.GetValues<TEnum>())
                .Select(converter.ToString)
                .ToList();
        }

        public static IBitmap? AsImage([AllowNull] this byte[] imageData) =>
            imageData == null || imageData.Length == 0 ? null : new Bitmap(new MemoryStream(imageData));
    }
}
