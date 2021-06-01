using System.Windows;

namespace Cineaste.Views
{
    public class NumberChangedEventArgs : RoutedEventArgs
    {
        public NumberChangedEventArgs(NumberTextBox numberTextBox, int oldValue, int newValue)
            : base(NumberTextBox.NumberChangedEvent, numberTextBox)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public int OldValue { get; }
        public int NewValue { get; }
    }

    public delegate void NumberChangedEventHandler(object sender, NumberChangedEventArgs e);
}
