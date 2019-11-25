using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MovieList.Views
{
    public static class Properties
    {
        public static readonly DependencyProperty TripleClickSelectAllProperty = DependencyProperty.RegisterAttached(
            "TripleClickSelectAll",
            typeof(bool),
            typeof(Properties),
            new PropertyMetadata(false, OnTripleClickSelectAllChanged));

        public static readonly DependencyProperty IsExternalProperty = DependencyProperty.RegisterAttached(
            "IsExternal",
            typeof(bool),
            typeof(Properties),
            new UIPropertyMetadata(false, OnIsExternalChanged));

        public static bool GetTripleClickSelectAll(DependencyObject element)
            => (bool)element.GetValue(TripleClickSelectAllProperty);

        public static void SetTripleClickSelectAll(DependencyObject element, bool value)
            => element.SetValue(TripleClickSelectAllProperty, value);

        public static bool GetIsExternal(DependencyObject obj)
            => (bool)obj.GetValue(IsExternalProperty);

        public static void SetIsExternal(DependencyObject obj, bool value)
            => obj.SetValue(IsExternalProperty, value);

        private static void OnTripleClickSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        private static void OnIsExternalChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is Hyperlink hyperlink)
            {
                if ((bool)args.NewValue)
                {
                    hyperlink.RequestNavigate += OnHyperlinkRequestNavigate;
                } else
                {
                    hyperlink.RequestNavigate -= OnHyperlinkRequestNavigate;
                }
            }
        }

        private static void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
