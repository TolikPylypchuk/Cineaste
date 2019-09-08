using System;
using System.Windows;

using HandyControl.Data;

namespace MovieList.Controls
{
    public class TextBox : HandyControl.Controls.TextBox
    {
        public static readonly DependencyProperty VerifyFuncProperty = DependencyProperty.Register(
            nameof(VerifyFunc),
            typeof(Func<string, OperationResult<bool>>),
            typeof(TextBox),
            new PropertyMetadata { PropertyChangedCallback = VerifyFuncChanged });

        public new Func<string, OperationResult<bool>> VerifyFunc
        {
            get => (Func<string, OperationResult<bool>>)this.GetValue(VerifyFuncProperty);
            set => this.SetValue(VerifyFuncProperty, value);
        }

        public override bool VerifyData()
            => !this.IsError && this.IsFocused || base.VerifyData();

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.VerifyData();
        }

        private static void VerifyFuncChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is HandyControl.Controls.TextBox textBox &&
                e.NewValue is Func<string, OperationResult<bool>> func)
            {
                textBox.VerifyFunc = func;
            }
        }
    }
}
