using System.Reactive.Linq;

using ReactiveUI;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class MainViewModel : ReactiveObject, IEnableLogger
    {
        public MainViewModel()
        {
            this.HomePageViewModel = new HomePageViewModel();

            this.OpenFile = ReactiveCommand.Create<OpenFileModel, OpenFileModel>(file => file);
            this.CloseFile = ReactiveCommand.Create<string, string>(file => file);

            this.HomePageViewModel.OpenFile
                .Merge(this.HomePageViewModel.CreateFile)
                .WhereNotNull()
                .Select(file => new OpenFileModel(file))
                .InvokeCommand(this.OpenFile);
        }

        public HomePageViewModel HomePageViewModel { get; set; }

        public ReactiveCommand<OpenFileModel, OpenFileModel> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }
    }
}
