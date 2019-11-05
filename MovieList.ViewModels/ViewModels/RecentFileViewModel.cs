using MovieList.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public class RecentFileViewModel : ReactiveObject
    {
        public RecentFileViewModel(RecentFile file, HomePageViewModel homePage)
        {
            this.File = file;
            this.HomePage = homePage;
        }

        [Reactive]
        public RecentFile File { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }

        public HomePageViewModel HomePage { get; }
    }
}
