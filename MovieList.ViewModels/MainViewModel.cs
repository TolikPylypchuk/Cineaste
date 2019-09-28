using ReactiveUI;

using Splat;

namespace MovieList.ViewModels
{
    public class MainViewModel : ReactiveObject, IEnableLogger
    {
        public MainViewModel()
        {
            this.OpenFile = ReactiveCommand.Create<string, string>(file =>
            {
                this.Log().Debug($"Opening a file: {file}");
                return file;
            });
        }

        public ReactiveCommand<string, string> OpenFile { get; }
    }
}
