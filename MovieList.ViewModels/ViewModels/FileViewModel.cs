using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

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

            Observable.FromAsync(kindService.GetAllKindsAsync)
                .Select(kinds => kinds.ToList())
                .Subscribe(kinds =>
                {
                    this.List = new ListViewModel(this.FileName, kinds);
                    this.Content ??= this.List;
                    this.kindsSource.AddOrUpdate(kinds);
                });

            this.SwitchToList = ReactiveCommand.Create(() => { this.Content = this.List; });
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.Create(() => { this.Content = this.Settings; });

            this.WhenAnyValue(vm => vm.ListName)
                .BindTo(this.Header, h => h.ListName);

            this.kindsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public FileHeaderViewModel Header { get; }

        [Reactive]
        public ReactiveObject Content { get; set; } = null!;

        public ListViewModel List { get; private set; } = null!;
        public SettingsViewModel Settings { get; private set; }

        public ReadOnlyObservableCollection<Kind> Kinds
            => this.kinds;

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }
    }
}
