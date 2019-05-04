using System.Windows;

namespace MovieList.Views
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(object),
            typeof(BindingProxy),
            new UIPropertyMetadata(null));

        public object Data
        {
            get => this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
            => new BindingProxy();
    }
}
