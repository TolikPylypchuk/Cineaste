using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using MovieList.Validation;

using ReactiveUI.Validation.Helpers;

namespace MovieList.Views
{
    public static class ViewExtensions
    {
        public static IDisposable ValidateWith(this FrameworkElement element, ValidationHelper rule)
            => ValidationSubscriber.Create(element, rule);

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
    }
}
