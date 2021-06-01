using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cineaste.Views
{
    public sealed class NumberTextBox : TextBox, IDisposable
    {
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            nameof(Number), typeof(int), typeof(NumberTextBox), new PropertyMetadata(default(int), OnNumberChanged));

        public static readonly RoutedEvent NumberChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(NumberChanged), RoutingStrategy.Direct, typeof(NumberChangedEventHandler), typeof(NumberTextBox));

        public NumberTextBox()
        {
            this.MaxLength = 9;
            DataObject.AddPastingHandler(this, this.DataObjectPasting);
        }

        public int Number
        {
            get => (int)this.GetValue(NumberProperty);
            set => this.SetValue(NumberProperty, value);
        }

        public void Dispose() =>
            DataObject.RemovePastingHandler(this, this.DataObjectPasting);

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.Number = !String.IsNullOrEmpty(this.Text) ? Int32.Parse(this.Text) : 0;
            base.OnTextChanged(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !this.IsTextNumeric(e.Text);
            base.OnPreviewTextInput(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
            base.OnPreviewKeyDown(e);
        }

        private static void OnNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is NumberTextBox numberTextBox && e.OldValue is int oldValue && e.NewValue is int newValue)
            {
                numberTextBox.Text = newValue != default ? newValue.ToString() : String.Empty;
                numberTextBox.RaiseEvent(new NumberChangedEventArgs(numberTextBox, oldValue, newValue));
            }
        }

        private void DataObjectPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string input = (string)e.DataObject.GetData(typeof(string));
                if (!this.IsTextNumeric(input.Trim()))
                {
                    e.CancelCommand();
                }
            } else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextNumeric(string text) =>
            text.All(Char.IsDigit);

        public event NumberChangedEventHandler NumberChanged
        {
            add => this.AddHandler(NumberChangedEvent, value);
            remove => this.RemoveHandler(NumberChangedEvent, value);
        }
    }
}
