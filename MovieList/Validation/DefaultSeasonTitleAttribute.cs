using System.ComponentModel.DataAnnotations;

using MovieList.Properties;

namespace MovieList.Validation
{
    public class DefaultSeasonTitleAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
            => value is string title && title.Contains(Messages.DefaultSeasonNumberPlaceholder);
    }
}
