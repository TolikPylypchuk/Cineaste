using ReactiveUI;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class MainViewModel : ReactiveObject, IEnableLogger
    {
        public MainViewModel()
        {
            this.HomePageViewModel = new HomePageViewModel();

            this.OpenFile = ReactiveCommand.Create<string, string>(file => file);
            this.CloseFile = ReactiveCommand.Create<string, string>(file => file);
        }

        public HomePageViewModel HomePageViewModel { get; set; }

        public ReactiveCommand<string, string> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }
    }
}
