using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public sealed class FileViewModel : ReactiveObject
    {
        public FileViewModel(string fileName, string listName)
        {
            this.FileName = fileName;
            this.ListName = listName;

            this.Header = new FileHeaderViewModel(FileName, ListName);

            this.WhenAnyValue(vm => vm.ListName)
                .BindTo(this.Header, h => h.ListName);
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public FileHeaderViewModel Header { get; }
    }
}
