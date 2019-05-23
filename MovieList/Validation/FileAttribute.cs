using System.ComponentModel.DataAnnotations;
using System.IO;

using MovieList.Properties;

namespace MovieList.Validation
{
    public class FileAttribute : ValidationAttribute
    {
        public FileAttribute()
        {
            this.ErrorMessageResourceName = "InvalidPath";
            this.ErrorMessageResourceType = typeof(Messages);
        }

        public override bool IsValid(object value)
            => value == null ||
                (value is string path &&
                 path.Length != 0 &&
                 path.IndexOfAny(Path.GetInvalidPathChars()) == -1);
    }
}
