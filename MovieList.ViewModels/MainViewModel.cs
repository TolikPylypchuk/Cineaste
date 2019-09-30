using ReactiveUI;

using Splat;

namespace MovieList.ViewModels
{
    public class MainViewModel : ReactiveObject, IEnableLogger
    {
        public MainViewModel()
        {
            this.OpenFile = ReactiveCommand.Create<string, string>(file => file);
            this.CloseFile = ReactiveCommand.Create<string, string>(file => file);
        }

        public ReactiveCommand<string, string> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }
    }
}
