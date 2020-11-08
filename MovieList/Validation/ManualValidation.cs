using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MovieList.Validation
{
    public static class ManualValidation
    {
        private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached(
            "Dummy", typeof(object), typeof(ManualValidation), new PropertyMetadata(null));

        public static void MarkInvalid(FrameworkElement element, object errorContent)
        {
            var binding = new Binding(nameof(Control.Tag)) { Source = element, Mode = BindingMode.OneWayToSource };

            BindingOperations.SetBinding(element, DummyProperty, binding);

            var bindingExpression = element.GetBindingExpression(DummyProperty);

            var validationError = new ValidationError(new DummyValidationRule(), binding, errorContent, null);

            System.Windows.Controls.Validation.MarkInvalid(
                bindingExpression ?? throw new InvalidOperationException(), validationError);
        }

        public static void ClearValidation(FrameworkElement element) =>
            BindingOperations.ClearBinding(element, DummyProperty);

        private class DummyValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
            throw new NotSupportedException("This is a dummy validation rule");
        }
    }
}
