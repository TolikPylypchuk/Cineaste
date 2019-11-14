using ReactiveUI;

namespace MovieList.ViewModels
{
    public sealed class SettingsViewModel : ReactiveObject
    {
        public SettingsViewModel(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; }
    }
}
