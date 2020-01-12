using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;
using MovieList.Data.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class FileViewModel : ReactiveObject
    {
        private readonly SourceCache<Kind, int> kindsSource;
        private readonly ReadOnlyObservableCollection<Kind> kinds;

        public FileViewModel(string fileName, string listName, IKindService? kindService = null)
        {
            this.FileName = fileName;
            this.ListName = listName;

            kindService ??= Locator.Current.GetService<IKindService>(fileName);

            this.Header = new FileHeaderViewModel(FileName, ListName);
            this.Settings = new SettingsViewModel(this.FileName);

            this.kindsSource = new SourceCache<Kind, int>(kind => kind.Id);

            this.kindsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            Observable.FromAsync(() => Task.Run(kindService.GetAllKindsAsync))
                .Do(this.kindsSource.AddOrUpdate)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(kinds =>
                {
                    this.MainContent = new FileMainContentViewModel(this.FileName, this.Kinds);
                    this.Content ??= this.MainContent;
                });

            this.SwitchToList = ReactiveCommand.Create(() => { this.Content = this.MainContent; });
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.Create(() => { this.Content = this.Settings; });

            this.WhenAnyValue(vm => vm.ListName)
                .BindTo(this.Header, h => h.ListName);
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public FileHeaderViewModel Header { get; }

        [Reactive]
        public ReactiveObject Content { get; set; } = null!;

        public FileMainContentViewModel MainContent { get; private set; } = null!;
        public SettingsViewModel Settings { get; private set; }

        public ReadOnlyObservableCollection<Kind> Kinds
            => this.kinds;

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }
    }
}
