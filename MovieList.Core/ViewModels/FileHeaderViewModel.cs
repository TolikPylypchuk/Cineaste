using System.Reactive;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public class FileHeaderViewModel : ReactiveObject
    {
        public FileHeaderViewModel(string fileName, string listName)
        {
            this.FileName = fileName;
            this.ListName = listName;

            this.Close = ReactiveCommand.Create<Unit, string>(_ => this.FileName);
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public ReactiveCommand<Unit, string> Close { get; }
    }
}
