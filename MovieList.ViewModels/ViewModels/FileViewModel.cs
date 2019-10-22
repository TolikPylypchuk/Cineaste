using ReactiveUI;

namespace MovieList.ViewModels
{
    public sealed class FileViewModel : ReactiveObject
    {
        public FileViewModel(string fileName, string listName)
        {
            this.FileName = fileName;
            this.ListName = listName;
        }

        public string FileName { get; }
        public string ListName { get; set; }
    }
}
