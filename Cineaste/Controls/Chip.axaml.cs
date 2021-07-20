using System;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

using Cineaste.Core;

namespace Cineaste.Controls
{
    public partial class Chip : UserControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<Chip, string>(nameof(Text));

        public static readonly StyledProperty<Brush> TagBrushProperty =
            AvaloniaProperty.Register<Chip, Brush>(nameof(TagBrush));

        public static readonly StyledProperty<bool> IsClickableProperty =
            AvaloniaProperty.Register<Chip, bool>(nameof(IsClickable));

        public static readonly StyledProperty<bool> IsDeletableProperty =
            AvaloniaProperty.Register<Chip, bool>(nameof(IsDeletable));

        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<Chip, RoutedEventArgs>(nameof(Click), RoutingStrategies.Direct);

        public static readonly RoutedEvent<RoutedEventArgs> DeletedEvent =
            RoutedEvent.Register<Chip, RoutedEventArgs>(nameof(Deleted), RoutingStrategies.Direct);

        public Chip()
        {
            this.DataContext = this;
            this.InitializeComponent();

            this.MainBorder.GetObservable(PointerReleasedEvent)
                .Where(_ => this.IsClickable)
                .Subscribe(_ => this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ClickEvent, Source = this }));

            this.GetObservable(IsClickableProperty)
                .Select(clickable => clickable ? StandardCursorType.Hand : StandardCursorType.Arrow)
                .Select(type => new Cursor(type))
                .Subscribe(cursor =>
                    this.MainBorder.Cursor = this.ColorTagBorder.Cursor = this.TextBlock.Cursor = cursor);

            this.DeleteButton.GetObservable(Button.ClickEvent)
                .Subscribe(_ => this.RaiseEvent(new RoutedEventArgs { RoutedEvent = DeletedEvent, Source = this }));
        }

        public string Text
        {
            get => this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public Brush TagBrush
        {
            get => this.GetValue(TagBrushProperty);
            set => this.SetValue(TagBrushProperty, value);
        }

        public bool IsClickable
        {
            get => this.GetValue(IsClickableProperty);
            set => this.SetValue(IsClickableProperty, value);
        }

        public bool IsDeletable
        {
            get => this.GetValue(IsDeletableProperty);
            set => this.SetValue(IsDeletableProperty, value);
        }

        public event EventHandler<RoutedEventArgs> Click
        {
            add => this.AddHandler(ClickEvent, value);
            remove => this.RemoveHandler(ClickEvent, value);
        }

        public event EventHandler<RoutedEventArgs> Deleted
        {
            add => this.AddHandler(DeletedEvent, value);
            remove => this.RemoveHandler(DeletedEvent, value);
        }
    }
}
