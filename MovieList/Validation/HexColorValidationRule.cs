using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

using MovieList.Properties;

namespace MovieList.Validation
{
    public class HexColorValidationRule : ValidationRule
    {
        private static readonly Regex HexString = new Regex(@"\A\b[0-9a-fA-F]+\b\Z", RegexOptions.Compiled);

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            => value is string color && this.IsArgbString(color)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, Messages.ValidationHexColorInvalid);

        private bool IsArgbString(string color)
            => color.Length == 9 && color.StartsWith('#') && HexString.IsMatch(color[1..]);
    }
}
