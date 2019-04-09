using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MovieList.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger log;

        public FileService(ILoggerFactory loggerFactory)
            => this.log = loggerFactory.CreateLogger(nameof(MovieList));

        public async ValueTask<bool> TryMoveFileAsync(string source, string destination)
        {
            if (source == destination)
            {
                return true;
            }

            try
            {
                await this.MoveFileAsync(source, destination);
                File.Delete(source);
            } catch (Exception exp)
            {
                this.log.LogError(exp.ToString());
                return false;
            }

            return true;
        }

        private async ValueTask MoveFileAsync(string source, string destination)
        {
            const int bufferSize = 4096;

            using var sourceStream = new FileStream(
                source,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            using var destinationStream = new FileStream(
                destination,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            await sourceStream.CopyToAsync(destinationStream);
        }
    }
}
