using System;
using System.Diagnostics.CodeAnalysis;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

using IconElement = FluentAvalonia.UI.Controls.IconElement;

namespace Cineaste.Controls
{
    public sealed class SymbolIcon : IconElement
    {
        static SymbolIcon() =>
            FontSizeProperty.OverrideDefaultValue<SymbolIcon>(18D);

        public static readonly StyledProperty<Symbol> SymbolProperty =
            AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol));

        public static readonly StyledProperty<double> FontSizeProperty =
            TextBlock.FontSizeProperty.AddOwner<SymbolIcon>();

        public static readonly StyledProperty<bool> UseFilledProperty =
            AvaloniaProperty.Register<SymbolIcon, bool>(nameof(UseFilled));

        private TextLayout? textLayout;

        public Symbol Symbol
        {
            get => this.GetValue(SymbolProperty);
            set => this.SetValue(SymbolProperty, value);
        }

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public bool UseFilled
        {
            get => GetValue(UseFilledProperty);
            set => SetValue(UseFilledProperty, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextBlock.FontSizeProperty ||
                change.Property == SymbolProperty ||
                change.Property == UseFilledProperty)
            {
                this.GenerateText();
                this.InvalidateMeasure();
            } else if (change.Property == TextBlock.ForegroundProperty)
            {
                this.GenerateText();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (textLayout == null)
            {
                this.GenerateText();
            }

            return this.textLayout.Size;
        }

        public override void Render(DrawingContext context)
        {
            if (textLayout == null)
            {
                this.GenerateText();
            }

            var dstRect = new Rect(this.Bounds.Size);
            using (context.PushClip(dstRect))
            using (context.PushPreTransform(Matrix.CreateTranslation(
                dstRect.Center.X - textLayout.Size.Width / 2, dstRect.Center.Y - textLayout.Size.Height / 2)))
            {
                textLayout.Draw(context);
            }
        }

        [MemberNotNull(nameof(textLayout))]
        private void GenerateText()
        {
            var code = this.UseFilled ? this.Symbol.ConvertToFilled() : (int)this.Symbol;
            var glyph = Char.ConvertFromUtf32(code).ToString();

            var typeFace = new Typeface(this.UseFilled
                ? new FontFamily("avares://FluentAvalonia/Fonts#FluentSystemIcons-Filled")
                : new FontFamily("avares://FluentAvalonia/Fonts#FluentSystemIcons-Regular"));

            this.textLayout = new TextLayout(glyph, typeFace, this.FontSize, this.Foreground, TextAlignment.Left);
        }
    }
}
