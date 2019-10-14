using MovieList.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public class RecentFileViewModel : ReactiveObject
    {
        public RecentFileViewModel(RecentFile file)
            => this.File = file;

        [Reactive]
        public RecentFile File { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }
    }
}
