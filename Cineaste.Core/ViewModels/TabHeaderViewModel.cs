using System.Reactive;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels
{
    public class TabHeaderViewModel : ReactiveObject
    {
        public TabHeaderViewModel(string fileName, string tabName)
        {
            this.FileName = fileName;
            this.TabName = tabName;

            this.Close = ReactiveCommand.Create<Unit, string>(_ => this.FileName);
        }

        public string FileName { get; }

        [Reactive]
        public string TabName { get; set; }

        public ReactiveCommand<Unit, string> Close { get; }
    }
}
