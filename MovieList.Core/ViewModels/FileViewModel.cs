using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using MovieList.Core.Data.Services;
using MovieList.Core.DialogModels;
using MovieList.Core.Models;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.Core.ViewModels
{
    public sealed class FileViewModel : ReactiveObject
    {
        private readonly SourceCache<Kind, int> kindsSource;
        private readonly ReadOnlyObservableCollection<Kind> kinds;

        private readonly SourceCache<Tag, int> tagsSource;
        private readonly ReadOnlyObservableCollection<Tag> tags;

        private readonly CompositeDisposable currentContentSubscriptions = new();
        private readonly ISettingsService settingsService;

        public FileViewModel(
            string fileName,
            string listName,
            ISettingsEntityService<Kind>? kindService = null,
            ISettingsEntityService<Tag>? tagService = null,
            ISettingsService? settingsService = null)
        {
            this.FileName = fileName;
            this.ListName = listName;

            kindService ??= Locator.Current.GetService<ISettingsEntityService<Kind>>(fileName);
            tagService ??= Locator.Current.GetService<ISettingsEntityService<Tag>>(fileName);
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>(fileName);

            this.Header = new TabHeaderViewModel(this.FileName, this.ListName);

            this.kindsSource = new SourceCache<Kind, int>(kind => kind.Id);
            this.tagsSource = new SourceCache<Tag, int>(tag => tag.Id);

            this.WhenAnyValue(vm => vm.Content)
                .Select(content => content is FileMainContentViewModel
                    ? this.WhenAnyValue(vm => vm.MainContent!.AreUnsavedChangesPresent)
                    : this.WhenAnyValue(vm => vm.Settings!.IsFormChanged))
                .Switch()
                .ToPropertyEx(this, vm => vm.AreUnsavedChangesPresent, initialValue: false);

            this.kindsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            this.tagsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.tags)
                .DisposeMany()
                .Subscribe();

            var kinds = kindService.GetAllInTaskPool()
                .Do(this.kindsSource.AddOrUpdate)
                .Discard()
                .ObserveOn(RxApp.MainThreadScheduler);

            var tags = tagService.GetAllInTaskPool()
                .Do(this.tagsSource.AddOrUpdate)
                .Discard()
                .ObserveOn(RxApp.MainThreadScheduler);

            kinds.CombineLatest(tags, (k, t) => Unit.Default)
                .SubscribeAsync(this.SwitchCurrentContentToMain);

            this.SwitchToList = ReactiveCommand.CreateFromObservable(this.OnSwitchToList);
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.CreateFromObservable(this.OnSwitchToSettings);
            this.UpdateSettings = ReactiveCommand.Create<SettingsModel, SettingsModel>(this.OnUpdateSettings);
            this.Save = ReactiveCommand.Create(() => { });

            this.WhenAnyValue(vm => vm.ListName)
                .BindTo(this.Header, h => h.TabName);
        }

        public string FileName { get; }

        [Reactive]
        public string ListName { get; set; }

        public TabHeaderViewModel Header { get; }

        [Reactive]
        public ReactiveObject Content { get; set; } = null!;

        public FileMainContentViewModel? MainContent { get; private set; }
        public SettingsFormViewModel? Settings { get; private set; }

        public ReadOnlyObservableCollection<Kind> Kinds =>
            this.kinds;

        public ReadOnlyObservableCollection<Tag> Tags =>
            this.tags;

        public bool AreUnsavedChangesPresent { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, Unit> SwitchToList { get; }
        public ReactiveCommand<Unit, Unit> SwitchToStats { get; }
        public ReactiveCommand<Unit, Unit> SwitchToSettings { get; }
        public ReactiveCommand<SettingsModel, SettingsModel> UpdateSettings { get; }
        public ReactiveCommand<Unit, Unit> Save { get; }

        private SettingsModel OnUpdateSettings(SettingsModel settingsModel)
        {
            this.kindsSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(settingsModel.Kinds);
            });

            this.tagsSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(settingsModel.Tags);
            });

            this.Header.TabName = settingsModel.Settings.ListName;

            return settingsModel;
        }

        private IObservable<Unit> OnSwitchToList()
        {
            var canSwitch = this.Settings?.IsFormChanged ?? false
                ? Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"))
                : Observable.Return(true);

            return canSwitch
                .ObserveOn(RxApp.MainThreadScheduler)
                .DoIfTrueAsync(this.SwitchCurrentContentToMain)
                .Discard();
        }

        private IObservable<Unit> SwitchCurrentContentToMain() =>
            this.settingsService.GetSettingsInTaskPool()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(settings =>
                {
                    this.currentContentSubscriptions.Clear();
                    this.Settings = null;

                    this.Content = this.MainContent = new FileMainContentViewModel(
                        this.FileName, this.Kinds, this.Tags, settings);

                    this.Save
                        .InvokeCommand(this.MainContent.TunnelSave)
                        .DisposeWith(this.currentContentSubscriptions);
                })
                .Discard();

        private IObservable<Unit> OnSwitchToSettings()
        {
            var observable = this.MainContent?.AreUnsavedChangesPresent ?? false
                ? Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"))
                : Observable.Return(true);

            return observable
                .SelectMany(canSwitch => canSwitch
                    ? this.settingsService.GetSettingsInTaskPool()
                    : Observable.Return<Settings?>(null))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(this.SwitchCurrentContentToSettings)
                .Discard();
        }

        private void SwitchCurrentContentToSettings(Settings? settings)
        {
            if (settings == null)
            {
                return;
            }

            this.currentContentSubscriptions.Clear();
            this.MainContent?.Dispose();
            this.MainContent = null;

            this.Content = this.Settings = new SettingsFormViewModel(this.FileName, settings, this.Kinds, this.Tags);

            this.Save
                .InvokeCommand(this.Settings.Save)
                .DisposeWith(this.currentContentSubscriptions);

            this.Settings.Save
                .InvokeCommand(this.UpdateSettings)
                .DisposeWith(this.currentContentSubscriptions);
        }
    }
}
