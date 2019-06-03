using System;
using System.ComponentModel.DataAnnotations;

using MovieList.Properties;

namespace MovieList.Validation
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public int Min { get; set; } = Int32.MinValue;
        public int Max { get; set; } = Int32.MaxValue;

        public override bool IsValid(object value)
            => value switch
            {
                int year => this.IsValid(year),
                string str when Int32.TryParse(str, out int year) => this.IsValid(year),
                null => true,
                _ => false
            };

        private bool IsValid(int year)
            => this.Min <= year && year <= this.Max;
    }
}
