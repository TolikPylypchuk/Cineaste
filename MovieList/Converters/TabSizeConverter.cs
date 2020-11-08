using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MovieList.Converters
{
    public sealed class TabSizeConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is TabControl tabControl)
            {
                double width = tabControl.ActualWidth / tabControl.Items.Count;
                return (width <= 1) ? 0 : (width - 1);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
