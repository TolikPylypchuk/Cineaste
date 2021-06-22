using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Cineaste.Core;
using Cineaste.Validation;

using ReactiveUI;
using ReactiveUI.Validation.Helpers;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Views
{
    public static class ViewExtensions
    {
        public static IDisposable ValidateWith(this FrameworkElement element, ValidationHelper rule) =>
            ValidationSubscriber.Create(element, rule);

        public static IDisposable ShowValidationMessage<TControl>(
            this TControl control,
            ValidationHelper rule,
            Expression<Func<TControl, string?>> property)
            where TControl : Control =>
            rule.ValidationChanged
                .Where(state => !state.IsValid)
                .Select(state => state.Text[0])
                .ObserveOnDispatcher()
                .BindTo(control, property);

        public static BitmapImage? AsImage([AllowNull] this byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            var image = new BitmapImage();

            using (var stream = new MemoryStream(imageData))
            {
                stream.Position = 0;

                image.BeginInit();

                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = stream;

                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        public static void AddEnumValues<TEnum>(this ComboBox comboBox, params TEnum[] values)
            where TEnum : struct, Enum
        {
            var converter = GetDefaultService<IEnumConverter<TEnum>>();

            (values.Length != 0 ? values : Enum.GetValues<TEnum>())
                .Select(converter.ToString)
                .ForEach(status => comboBox.Items.Add(status));
        }
    }
}
