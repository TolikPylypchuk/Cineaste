using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;

using Akavache;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.Data.Services;
using MovieList.Core.DialogModels;
using MovieList.Core.Models;
using MovieList.Core.Preferences;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data;
using MovieList.Data.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Core.Constants;
using static MovieList.Data.Constants;

namespace MovieList.Core.ViewModels
{
    public sealed class MainViewModel : ReactiveObject, IEnableLogger
    {
        private readonly IBlobCache store;
        private readonly IScheduler scheduler;

        private readonly SourceCache<FileViewModel, string> fileViewModelsSource;

        private readonly Dictionary<string, IDisposable> closeSubscriptions = new Dictionary<string, IDisposable>();
        private readonly CompositeDisposable preferencesSubscriptions = new CompositeDisposable();

        private readonly Subject<bool> showRecentFiles = new Subject<bool>();

        private readonly Version Version = Assembly.GetEntryAssembly()!.GetName()!.Version!;

        public MainViewModel(IBlobCache? store = null, IScheduler? scheduler = null)
        {
            this.store = store ?? Locator.Current.GetService<IBlobCache>(StoreKey);
            this.scheduler = scheduler ?? Scheduler.Default;

            this.HomePage = new HomePageViewModel(showRecentFiles);

            this.fileViewModelsSource = new SourceCache<FileViewModel, string>(x => x.FileName);

            this.fileViewModelsSource.Connect()
                .Bind(out var fileViewModels)
                .DisposeMany()
                .Subscribe();

            this.Files = fileViewModels;

            this.CreateFile = ReactiveCommand.CreateFromObservable<CreateFileModel, CreateFileModel?>(
                this.OnCreateFile);
            this.OpenFile = ReactiveCommand.CreateFromObservable<OpenFileModel, OpenFileModel?>(this.OnOpenFile);

            var canSave = this.CanSave();

            var canSaveAs = this.WhenAnyValue(vm => vm.SelectedItemIndex)
                .DistinctUntilChanged()
                .Select(index => index > 0 && index != this.Files.Count + 1)
                .CombineLatest(canSave, (a, b) => a && !b)
                .ObserveOn(RxApp.MainThreadScheduler);

            this.Save = ReactiveCommand.Create<Unit, int>(_ => this.SelectedItemIndex, canSave);
            this.SaveAs = ReactiveCommand.CreateFromObservable(this.OnSaveFileAs, canSaveAs);

            var canOpenSettings = this.WhenAnyValue(vm => vm.SelectedItemIndex)
                .DistinctUntilChanged()
                .Select(index => index > 0 && index != this.Files.Count + 1)
                .ObserveOn(RxApp.MainThreadScheduler);

            this.OpenSettings = ReactiveCommand.Create(() => { }, canOpenSettings);

            var canCloseCurrentTab = this.WhenAnyValue(vm => vm.SelectedItemIndex)
                .DistinctUntilChanged()
                .Select(index => index > 0)
                .ObserveOn(RxApp.MainThreadScheduler);

            this.CloseFile = ReactiveCommand.CreateFromObservable<string, Unit>(this.OnCloseFile);
            this.CloseCurrentTab = ReactiveCommand.Create<Unit, int>(_ => this.SelectedItemIndex, canCloseCurrentTab);

            this.Shutdown = ReactiveCommand.CreateFromObservable(this.OnShutdown);
            this.ShowAbout = ReactiveCommand.CreateFromObservable(() =>
                Dialog.ShowAbout.Handle(new AboutModel { Version = $"{this.Version.Major}.{this.Version.Minor}" }));

            this.OpenPreferences = ReactiveCommand.CreateFromObservable(this.OnOpenPreferences);
            this.ClosePreferences = ReactiveCommand.CreateFromObservable(this.OnClosePreferences);

            this.InitializeCommandDependencies();
        }

        public HomePageViewModel HomePage { get; set; }
        public ReadOnlyObservableCollection<FileViewModel> Files { get; }

        [Reactive]
        public PreferencesFormViewModel? Preferences { get; set; }

        [Reactive]
        public int SelectedItemIndex { get; set; }

        public ReactiveCommand<CreateFileModel, CreateFileModel?> CreateFile { get; }
        public ReactiveCommand<OpenFileModel, OpenFileModel?> OpenFile { get; }

        public ReactiveCommand<Unit, int> Save { get; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; }
        public ReactiveCommand<Unit, Unit> OpenSettings { get; }

        public ReactiveCommand<string, Unit> CloseFile { get; }
        public ReactiveCommand<Unit, int> CloseCurrentTab { get; }

        public ReactiveCommand<Unit, Unit> Shutdown { get; }
        public ReactiveCommand<Unit, Unit> ShowAbout { get; }

        public ReactiveCommand<Unit, Unit> OpenPreferences { get; }
        public ReactiveCommand<Unit, Unit> ClosePreferences { get; }

        private IObservable<CreateFileModel?> OnCreateFile(CreateFileModel model)
        {
            this.Log().Debug($"Creating a file: {model.File}");

            Locator.CurrentMutable.RegisterDatabaseServices(model.File);

            var preferences = Locator.Current.GetService<UserPreferences>().Defaults;

            var settings = new Settings(
                model.ListName,
                ListFileVersion,
                preferences.DefaultSeasonTitle,
                preferences.DefaultSeasonOriginalTitle,
                preferences.DefaultCultureInfo.ToString());

            return Locator.Current.GetService<IDatabaseService>(model.File)
                .CreateDatabaseInTaskPool(settings, preferences.DefaultKinds)
                .SelectMany(_ => this.GetSettings(model.File))
                .Do(dbSettings => Locator.CurrentMutable.RegisterConstant(dbSettings, model.File))
                .Do(_ => this.AddFile(model.File, model.ListName))
                .Select(_ => model);
        }

        private IObservable<OpenFileModel?> OnOpenFile(OpenFileModel model)
        {
            if (String.IsNullOrEmpty(model.File))
            {
                return Observable.Return(model);
            }

            int fileIndex = this.Files.TakeWhile(file => file.FileName != model.File).Count();

            if (fileIndex != this.Files.Count)
            {
                this.Log().Debug($"The file is already opened: {model.File}. Opening its tab");
                this.SetSelectedItemIndex(fileIndex + 1);
                return Observable.Return(model);
            }

            this.Log().Debug($"Opening a file: {model.File}");

            Locator.CurrentMutable.RegisterDatabaseServices(model.File);
            var settingsService = Locator.Current.GetService<ISettingsService>(model.File);
            Locator.CurrentMutable.RegisterConstant(settingsService.GetSettings(), model.File);

            return Locator.Current.GetService<IDatabaseService>(model.File)
                .ValidateDatabaseInTaskPool()
                .SelectMany(isFileValid =>
                {
                    if (!isFileValid)
                    {
                        this.Log().Debug($"Cancelling opening a file: {model.File}");
                        Locator.CurrentMutable.UnregisterDatabaseServices(model.File);
                        return Observable.Return<OpenFileModel?>(null);
                    }

                    return this.GetSettings(model.File)
                        .Do(settings => this.AddFile(model.File, settings.ListName))
                        .Select(_ => model);
                });
        }

        private IObservable<Unit> OnSaveFileAs()
        {
            var currentFile = this.Files[this.SelectedItemIndex - 1];

            return Dialog.SaveFile.Handle(Path.GetFileName(currentFile.FileName))
                .SelectMany(newFileName => newFileName != null
                    ? this.OnSaveFileAsCore(currentFile.FileName, newFileName)
                    : Observable.Return(Unit.Default));
        }

        private IObservable<Unit> OnSaveFileAsCore(string fileName, string newFileName)
        {
            File.Copy(fileName, newFileName);
            return this.OpenFile.Execute(new OpenFileModel(newFileName)).Discard();
        }

        private IObservable<Unit> OnCloseFile(string file)
        {
            var fileViewModel = this.Files.First(f => f.FileName == file);

            var observable = fileViewModel.AreUnsavedChangesPresent
                ? Dialog.Confirm.Handle(new ConfirmationModel("CloseFile"))
                : Observable.Return(true);

            return observable.SelectMany(canClose => canClose
                ? this.OnCloseFileCore(fileViewModel.FileName)
                : Observable.Return(Unit.Default));
        }

        private IObservable<Unit> OnCloseFileCore(string file)
        {
            this.Log().Debug($"Closing a file: {file}");

            int fileIndex = this.Files.TakeWhile(f => f.FileName != file).Count() + 1;
            int currentIndex = this.SelectedItemIndex;

            this.fileViewModelsSource.RemoveKey(file);
            this.closeSubscriptions[file].Dispose();
            this.closeSubscriptions.Remove(file);

            this.SetSelectedItemIndex(currentIndex == fileIndex ? fileIndex - 1 : currentIndex);

            return this.store.GetObject<UserPreferences>(PreferencesKey)
                .DoAsync(preferences => this.AddFileToRecent(preferences, file, true))
                .DoAsync(preferences => this.store.InsertObject(PreferencesKey, preferences).Eager())
                .Select(_ => file)
                .Do(Locator.CurrentMutable.UnregisterDatabaseServices)
                .Discard()
                .Eager();
        }

        private IObservable<Unit> OnShutdown()
            => this.store.GetObject<UserPreferences>(PreferencesKey)
                .DoAsync(preferences => this.Files
                    .Select(file => this.AddFileToRecent(preferences, file.FileName, false))
                    .Concat()
                    .LastOrDefaultAsync())
                .SelectMany(preferences => this.store.InsertObject(PreferencesKey, preferences).Eager())
                .Eager();

        private IObservable<Unit> OnOpenPreferences()
        {
            if (this.Preferences != null)
            {
                this.SetSelectedItemIndex(this.Files.Count + 1);
            }

            return this.store.GetObject<UserPreferences>(PreferencesKey)
                .Select(preferences => new PreferencesFormViewModel(preferences, this.store))
                .Do(this.OpenPreferencesForm)
                .Discard()
                .Eager();
        }

        private IObservable<Unit> OnClosePreferences()
        {
            var observable = this.Preferences?.IsFormChanged ?? false
                ? Dialog.Confirm.Handle(new ConfirmationModel("CloseForm"))
                : Observable.Return(true);

            return observable.Do(canClose =>
                {
                    if (canClose)
                    {
                        if (this.SelectedItemIndex > this.Files.Count)
                        {
                            this.SetSelectedItemIndex(this.Files.Count);
                        }

                        this.preferencesSubscriptions.Clear();
                        this.Preferences = null;
                    }
                })
                .Discard();
        }

        private void AddFile(string fileName, string listName)
        {
            var fileViewModel = new FileViewModel(fileName, listName);

            var subscriptions = new CompositeDisposable();

            fileViewModel.Header.Close
                .InvokeCommand(this.CloseFile)
                .DisposeWith(subscriptions);

            fileViewModel.UpdateSettings
                .Select(settingsModel => settingsModel.Settings.ListName)
                .Where(name => name != listName)
                .SubscribeAsync(name => this.UpdateRecentFile(fileName, name))
                .DisposeWith(subscriptions);

            this.closeSubscriptions.Add(fileName, subscriptions);

            this.fileViewModelsSource.AddOrUpdate(fileViewModel);

            this.SetSelectedItemIndex(this.Files.Count);
        }

        private IObservable<Unit> AddFileToRecent(UserPreferences preferences, string file, bool notifyHomePage)
        {
            if (!preferences.File.ShowRecentFiles)
            {
                return Observable.Return(Unit.Default);
            }

            var recentFile = preferences.File.RecentFiles.FirstOrDefault(f => f.Path == file);

            var newRecentFileObservable = recentFile != null
                ? Observable.Return(new RecentFile(recentFile.Name, recentFile.Path, this.scheduler.Now.LocalDateTime))
                    .Do(_ => preferences.File.RecentFiles.Remove(recentFile))
                : this.GetSettings(file)
                    .Select(settings => new RecentFile(settings.ListName, file, this.scheduler.Now.LocalDateTime));

            return newRecentFileObservable
                .Do(preferences.File.RecentFiles.Add)
                .SelectMany(newRecentFile => notifyHomePage
                    ? this.UpdateRecentFileInHomePage(recentFile, newRecentFile)
                    : Observable.Return(Unit.Default));
        }

        private IObservable<Unit> UpdateRecentFileInHomePage(RecentFile? oldRecentFile, RecentFile newRecentFile)
            => Observable.CombineLatest(
                    oldRecentFile != null
                        ? this.HomePage.RemoveRecentFile.Execute(oldRecentFile)
                        : Observable.Return(Unit.Default),
                    this.HomePage.AddRecentFile.Execute(newRecentFile))
                .Discard();

        private IObservable<Unit> UpdateRecentFile(string file, string newName)
            => this.store.GetObject<UserPreferences>(PreferencesKey)
                .Eager()
                .Select(preferences => new
                {
                    Preferences = preferences,
                    RecentFile = preferences.File.RecentFiles.FirstOrDefault(f => f.Path == file)
                })
                .Where(data => data.RecentFile != null)
                .Do(data => data.Preferences.File.RecentFiles.Remove(data.RecentFile))
                .DoAsync(data => this.HomePage.RemoveRecentFile.Execute(data.RecentFile))
                .Select(data => new
                {
                    data.Preferences,
                    NewRecentFile = new RecentFile(newName, file, data.RecentFile.Closed)
                })
                .Do(data => data.Preferences.File.RecentFiles.Add(data.NewRecentFile))
                .DoAsync(data => this.HomePage.AddRecentFile.Execute(data.NewRecentFile))
                .SelectMany(data => this.store.InsertObject(PreferencesKey, data.Preferences).Eager());

        private IObservable<Settings> GetSettings(string file)
            => Locator.Current.GetService<ISettingsService>(file).GetSettingsInTaskPool();

        private void OpenPreferencesForm(PreferencesFormViewModel form)
        {
            form.Save
                .Select(preferences => preferences.File.ShowRecentFiles)
                .DistinctUntilChanged()
                .Subscribe(this.showRecentFiles)
                .DisposeWith(this.preferencesSubscriptions);

            form.Header.Close
                .Discard()
                .InvokeCommand(this.ClosePreferences)
                .DisposeWith(this.preferencesSubscriptions);

            this.Preferences = form;

            this.SetSelectedItemIndex(this.Files.Count + 1);
        }

        private IObservable<bool> CanSave()
        {
            var autoRefreshedFiles = this.Files.ToObservableChangeSet()
                .AutoRefresh(file => file.AreUnsavedChangesPresent)
                .ToCollection();

            var canSaveFile = this.WhenAnyValue(vm => vm.SelectedItemIndex)
                .DistinctUntilChanged()
                .Select(index => index > 0 && index != this.Files.Count + 1
                    ? autoRefreshedFiles.Select(files =>
                        this.SelectedItemIndex > 0 &&
                        this.SelectedItemIndex != this.Files.Count + 1 &&
                        files.ToList()[this.SelectedItemIndex - 1].AreUnsavedChangesPresent)
                    : Observable.Return(false))
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler);

            var canSavePreferences = this.WhenAnyValue(vm => vm.SelectedItemIndex)
                .DistinctUntilChanged()
                .Select(index => index == this.Files.Count + 1)
                .CombineLatest(
                    this.WhenAnyValue(vm => vm.Preferences)
                        .Select(p => p != null ? p.Save.CanExecute : Observable.Return(false))
                        .Switch(),
                    (a, b) => a && b)
                .ObserveOn(RxApp.MainThreadScheduler);

            return Observable.CombineLatest(canSaveFile, canSavePreferences, (a, b) => a || b)
                .ObserveOn(RxApp.MainThreadScheduler);
        }

        private void InitializeCommandDependencies()
        {
            this.HomePage.CreateFile
                .WhereNotNull()
                .InvokeCommand(this.CreateFile);

            this.HomePage.OpenFile
                .WhereNotNull()
                .Select(file => new OpenFileModel(file))
                .InvokeCommand(this.OpenFile);

            this.Save
                .SelectMany(index => index != this.Files.Count + 1
                    ? this.Files[index - 1].Save.Execute()
                    : this.Preferences!.Save.Execute().Discard())
                .Subscribe();

            this.OpenSettings
                .SelectMany(() => this.Files[this.SelectedItemIndex - 1].SwitchToSettings.Execute())
                .Subscribe();

            this.CloseCurrentTab
                .SelectMany(index => index != this.Files.Count + 1
                    ? this.CloseFile.Execute(this.Files[index - 1].FileName)
                    : this.ClosePreferences.Execute())
                .Subscribe();
        }

        private void SetSelectedItemIndex(int index)
            => RxApp.MainThreadScheduler.Schedule(() => this.SelectedItemIndex = index);
    }
}
