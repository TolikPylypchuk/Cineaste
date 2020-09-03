using System.Globalization;
using System.Windows.Controls;

using MovieList.Core.Validation;
using MovieList.Properties;

namespace MovieList.Validation
{
    public class HexColorValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            => value is string color && color.IsArgbString()
                ? ValidationResult.ValidResult
                : new ValidationResult(false, Messages.ValidationHexColorInvalid);
    }
}
