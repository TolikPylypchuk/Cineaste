using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Cineaste.Controls
{
    public partial class FluentIcon : UserControl
    {
        public static readonly StyledProperty<Drawing?> IconProperty =
            AvaloniaProperty.Register<FluentIcon, Drawing?>(nameof(Icon));

        public FluentIcon()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public Drawing? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }
    }
}
