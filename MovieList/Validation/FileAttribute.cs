using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MovieList.Validation
{
    public class FileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
            => value == null ||
                (value is string path &&
                 path.Length != 0 &&
                 path.IndexOfAny(Path.GetInvalidPathChars()) == -1);
    }
}
