using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Data.Services;
using MovieList.DialogModels;
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
        private readonly CompositeDisposable settingsFormSubscriptions = new CompositeDisposable();
        private readonly ISettingsService settingsService;

        public FileViewModel(
            string fileName,
            string listName,
            IKindService? kindService = null,
            ISettingsService? settingsService = null)
        {
            this.FileName = fileName;
            this.ListName = listName;

            kindService ??= Locator.Current.GetService<IKindService>(fileName);
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>(fileName);

            this.Header = new TabHeaderViewModel(this.FileName, this.ListName);

            this.kindsSource = new SourceCache<Kind, int>(kind => kind.Id);

            this.kindsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.kinds)
                .DisposeMany()
                .Subscribe();

            kindService.GetAllKindsInTaskPool()
                .Do(this.kindsSource.AddOrUpdate)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(kinds =>
                {
                    this.MainContent = new FileMainContentViewModel(this.FileName, this.Kinds);
                    this.Content ??= this.MainContent;
                });

            this.SwitchToList = ReactiveCommand.CreateFromObservable(this.OnSwitchToList);
            this.SwitchToStats = ReactiveCommand.Create(() => { });
            this.SwitchToSettings = ReactiveCommand.CreateFromObservable(this.OnSwitchToSettings);
            this.UpdateSettings = ReactiveCommand.Create<SettingsModel, SettingsModel>(this.OnUpdateSettings);

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
                .DoIfTrue(() =>
                {
                    this.settingsFormSubscriptions.Clear();
                    this.Content = this.MainContent = new FileMainContentViewModel(this.FileName, this.Kinds);
                })
                .Discard();
        }

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
                .Do(settings =>
                {
                    if (settings != null)
                    {
                        this.MainContent?.Dispose();
                        this.Content = this.Settings = new SettingsFormViewModel(this.FileName, settings, this.Kinds);
                        this.Settings.Save
                            .InvokeCommand(this.UpdateSettings)
                            .DisposeWith(this.settingsFormSubscriptions);
                    }
                })
                .Discard();
        }
    }
}
