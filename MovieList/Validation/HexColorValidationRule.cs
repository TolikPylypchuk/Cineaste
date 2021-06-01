using System.Globalization;
using System.Windows.Controls;

using Cineaste.Core.Validation;
using Cineaste.Properties;

namespace Cineaste.Validation
{
    public class HexColorValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
            value is string color && color.IsArgbString()
                ? ValidationResult.ValidResult
                : new ValidationResult(false, Messages.ValidationHexColorInvalid);
    }
}
