using ReactiveUI;

namespace MovieList.ViewModels
{
    public sealed class ListViewModel : ReactiveObject
    {
        public ListViewModel(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; }
    }
}
