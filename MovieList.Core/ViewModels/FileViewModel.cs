using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Resources;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.Models;
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
            List<Kind> kinds,
            Settings settings,
            IKindService? kindService = null,
            ISettingsService? settingsService = null,
            ResourceManager? resourceManager = null,
            IScheduler? scheduler = null)
        {
            this.FileName = fileName;
            this.ListName = listName;

            kindService ??= Locator.Current.GetService<IKindService>(fileName);
            settingsService ??= Locator.Current.GetService<ISettingsService>(fileName);

            this.Header = new FileHeaderViewModel(this.FileName, this.ListName);

            this.kindsSource = new SourceCache<Kind, int>(kind => kind.Id);

            this.kindsSource.Connect()
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            this.kindsSource.AddOrUpdate(kinds);

            this.MainContent = new FileMainContentViewModel(this.FileName, this.Kinds);
            this.Content = this.MainContent;

            this.Settings = new SettingsFormViewModel(
                this.FileName, settings, kinds, settingsService, kindService, resourceManager, scheduler);

            this.SwitchToList = ReactiveCommand.Create(() => { this.Content = this.MainContent; });
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.Create(() => { this.Content = this.Settings; });
            this.UpdateSettings = ReactiveCommand.Create<SettingsModel, SettingsModel>(this.OnUpdateSettings);

            this.Settings.Save.InvokeCommand(this.UpdateSettings);

            this.WhenAnyValue(vm => vm.ListName)
                .BindTo(this.Header, h => h.ListName);
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public FileHeaderViewModel Header { get; }

        [Reactive]
        public ReactiveObject Content { get; set; }

        public FileMainContentViewModel MainContent { get; private set; }
        public SettingsFormViewModel Settings { get; private set; }

        public ReadOnlyObservableCollection<Kind> Kinds
            => this.kinds;

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }
        public ReactiveCommand<SettingsModel, SettingsModel> UpdateSettings { get; }

        private SettingsModel OnUpdateSettings(SettingsModel settingsModel)
        {
            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(settingsModel.Kinds);
            });

            this.Header.ListName = settingsModel.Settings.ListName;

            return settingsModel;
        }
    }
}
