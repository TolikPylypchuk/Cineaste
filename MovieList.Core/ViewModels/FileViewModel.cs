using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.ViewModels.Forms.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels
{
    public sealed class FileViewModel : ReactiveObject
    {
        private readonly SourceCache<Kind, int> kindsSource;
        private readonly ReadOnlyObservableCollection<Kind> kinds;

        public FileViewModel(
            string fileName,
            string listName,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null,
            IKindService? kindService = null,
            ISettingsService? settingsService = null)
        {
            this.FileName = fileName;
            this.ListName = listName;

            kindService ??= Locator.Current.GetService<IKindService>(fileName);
            settingsService ??= Locator.Current.GetService<ISettingsService>(fileName);

            this.Header = new FileHeaderViewModel(this.FileName, this.ListName);

            this.kindsSource = new SourceCache<Kind, int>(kind => kind.Id);

            this.kindsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            var getKinds = Observable.FromAsync(() => Task.Run(kindService.GetAllKindsAsync))
                .Do(this.kindsSource.AddOrUpdate)
                .Publish();

            getKinds
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(kinds =>
                {
                    this.MainContent = new FileMainContentViewModel(this.FileName, this.Kinds);
                    this.Content ??= this.MainContent;
                });

            Observable.FromAsync(() => Task.Run(settingsService.GetSettingsAsync))
                .CombineLatest(getKinds, (settings, kinds) => (Settings: settings, Kinds: kinds))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(data =>
                {
                    this.Settings = new SettingsFormViewModel(
                        this.FileName,
                        data.Settings,
                        data.Kinds,
                        resourceManager,
                        scheduler,
                        settingsService, kindService);

                    this.Settings.Save
                        .Select(settingsModel => settingsModel.Kinds)
                        .Subscribe(this.UpdateKinds);
                });

            getKinds.Connect();

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
        public SettingsFormViewModel Settings { get; private set; } = null!;

        public ReadOnlyObservableCollection<Kind> Kinds
            => this.kinds;

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }

        private void UpdateKinds(List<Kind> kinds)
            => this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(kinds);
            });
    }
}
