using System.Threading.Tasks;

namespace MovieList.Services
{
    public interface IFileService
    {
        ValueTask<bool> TryMoveFileAsync(string source, string destination);
    }
}
