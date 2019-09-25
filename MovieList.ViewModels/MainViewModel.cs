using ReactiveUI;

namespace MovieList.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            this.OpenFile = ReactiveCommand.Create<string, string>(file => file);
        }

        public ReactiveCommand<string, string> OpenFile { get; }
    }
}
