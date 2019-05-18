using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using HandyControl.Controls;
using HandyControl.Tools;

namespace MovieList
{
    public static class Util
    {
        public static int GetHashCode(params object[] properties)
            => properties.Select(p => p.GetHashCode()).Aggregate(17, (acc, hash) => unchecked(acc * 23 + hash));

        public static int BinarySearchIndexOf<T>(IList<T> list, T value, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = comparer.Compare(value, list[middle]);

                if (comparisonResult == 0)
                {
                    return middle;
                } else if (comparisonResult < 0)
                {
                    upper = middle - 1;
                } else
                {
                    lower = middle + 1;
                }
            }

            return ~lower;
        }

        public static void OpenColorPickerPopup(FrameworkElement? parent, string color, Action<Color> setColor)
        {
            var picker = SingleOpenHelper.CreateControl<ColorPicker>();

            picker.Loaded += (sender, e) =>
                picker.SelectedBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(color);

            var window = new PopupWindow
            {
                PopupElement = picker
            };

            picker.SelectedColorChanged += (sender, e) =>
            {
                setColor(e.Info);
                window.Close();
            };

            picker.Canceled += (sender, e) => window.Close();

            window.Show(parent, false);
        }
    }
}
