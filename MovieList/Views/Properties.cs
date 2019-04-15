using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieList.Views
{
    public static class Properties
    {
        public static readonly DependencyProperty TripleClickSelectAllProperty = DependencyProperty.RegisterAttached(
            "TripleClickSelectAll", typeof(bool), typeof(Properties), new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && e.NewValue is bool enable)
            {
                if (enable)
                {
                    textBox.PreviewMouseLeftButtonDown += OnTextBoxMouseDown;
                } else
                {
                    textBox.PreviewMouseLeftButtonDown -= OnTextBoxMouseDown;
                }
            }
        }

        private static void OnTextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 3 && sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        public static void SetTripleClickSelectAll(DependencyObject element, bool value)
            => element.SetValue(TripleClickSelectAllProperty, value);

        public static bool GetTripleClickSelectAll(DependencyObject element)
            => (bool)element.GetValue(TripleClickSelectAllProperty);
    }
}
