using System.Reactive;
using System.Reactive.Disposables;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public sealed class FileViewModel : ReactiveObject, IActivatableViewModel
    {
        public FileViewModel(string fileName, string listName)
        {
            this.FileName = fileName;
            this.ListName = listName;

            this.Header = new FileHeaderViewModel(FileName, ListName);

            this.List = new ListViewModel(this.FileName);
            this.Settings = new SettingsViewModel(this.FileName);

            this.Content = this.List;

            this.SwitchToList = ReactiveCommand.Create(() => { this.Content = this.List; });
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.Create(() => { this.Content = this.Settings; });

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(vm => vm.ListName)
                    .BindTo(this.Header, h => h.ListName)
                    .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public FileHeaderViewModel Header { get; }

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }

        [Reactive]
        public ReactiveObject Content { get; set; }

        public ListViewModel List { get; }
        public SettingsViewModel Settings { get; }
    }
}
