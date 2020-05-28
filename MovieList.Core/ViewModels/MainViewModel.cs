using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Akavache;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Services;
using MovieList.DialogModels;
using MovieList.Models;
using MovieList.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Constants;
using static MovieList.Data.Constants;

namespace MovieList.ViewModels
{
    public sealed class MainViewModel : ReactiveObject, IEnableLogger
    {
        private readonly IBlobCache store;
        private readonly IScheduler scheduler;

        private readonly SourceCache<FileViewModel, string> fileViewModelsSource;

        private readonly Dictionary<string, IDisposable> closeSubscriptions = new Dictionary<string, IDisposable>();

        public MainViewModel(IBlobCache? store = null, IScheduler? scheduler = null)
        {
            this.store = store ?? Locator.Current.GetService<IBlobCache>(StoreKey);
            this.scheduler = scheduler ?? Scheduler.Default;

            this.HomePage = new HomePageViewModel();

            this.fileViewModelsSource = new SourceCache<FileViewModel, string>(x => x.FileName);

            this.fileViewModelsSource.Connect()
                .Bind(out var fileViewModels)
                .DisposeMany()
                .Subscribe();

            this.Files = fileViewModels;

            this.CreateFile = ReactiveCommand.Create<CreateFileModel, CreateFileModel?>(this.OnCreateFile);
            this.OpenFile = ReactiveCommand.Create<OpenFileModel, OpenFileModel?>(this.OnOpenFile);
            this.CloseFile = ReactiveCommand.CreateFromTask<string, string>(this.OnCloseFileAsync);
            this.Shutdown = ReactiveCommand.CreateFromTask(this.OnShutdownAsync);
            this.ShowAbout = ReactiveCommand.CreateFromTask(async () => await Dialog.ShowMessage.Handle(
                new MessageModel("AboutText", "AboutTitle")));

            this.HomePage.CreateFile
                .WhereNotNull()
                .InvokeCommand(this.CreateFile);

            this.HomePage.OpenFile
                .WhereNotNull()
                .Select(file => new OpenFileModel(file))
                .InvokeCommand(this.OpenFile);
        }

        public HomePageViewModel HomePage { get; set; }
        public ReadOnlyObservableCollection<FileViewModel> Files { get; }

        [Reactive]
        public int SelectedItemIndex { get; set; }

        public ReactiveCommand<CreateFileModel, CreateFileModel?> CreateFile { get; }
        public ReactiveCommand<OpenFileModel, OpenFileModel?> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }
        public ReactiveCommand<Unit, Unit> Shutdown { get; }
        public ReactiveCommand<Unit, Unit> ShowAbout { get; }

        private CreateFileModel? OnCreateFile(CreateFileModel model)
        {
            this.Log().Debug($"Creating a file: {model.File}");

            Locator.CurrentMutable.RegisterDatabaseServices(model.File);

            var preferences = Locator.Current.GetService<UserPreferences>().Defaults;

            var settings = new Settings(
                model.ListName,
                ListFileVersion,
                preferences.DefaultSeasonTitle,
                preferences.DefaultSeasonOriginalTitle,
                preferences.DefaultCultureInfo);

            Locator.Current.GetService<IDatabaseService>(model.File).CreateDatabase(settings, preferences.DefaultKinds);

            var settingsService = Locator.Current.GetService<ISettingsService>(model.File);
            Locator.CurrentMutable.RegisterConstant(settingsService.GetSettings(), model.File);

            this.AddFile(model.File, model.ListName);

            return model;
        }

        private OpenFileModel? OnOpenFile(OpenFileModel model)
        {
            if (String.IsNullOrEmpty(model.File))
            {
                return model;
            }

            int fileIndex = this.Files.TakeWhile(file => file.FileName != model.File).Count();

            if (fileIndex != this.Files.Count)
            {
                this.Log().Debug($"The file is already opened: {model.File}. Opening its tab");
                this.SelectedItemIndex = fileIndex + 1;
                return model;
            }

            this.Log().Debug($"Opening a file: {model.File}");

            Locator.CurrentMutable.RegisterDatabaseServices(model.File);
            var settingsService = Locator.Current.GetService<ISettingsService>(model.File);
            Locator.CurrentMutable.RegisterConstant(settingsService.GetSettings(), model.File);

            bool isFileValid = Locator.Current.GetService<IDatabaseService>(model.File).ValidateDatabase();

            if (!isFileValid)
            {
                this.Log().Debug($"Cancelling opening a file: {model.File}");
                Locator.CurrentMutable.UnregisterDatabaseServices(model.File);
                return null;
            }

            var settings = Locator.Current.GetService<ISettingsService>(model.File).GetSettings();

            this.AddFile(model.File, settings.ListName);

            return model;
        }

        private async Task<string> OnCloseFileAsync(string file)
        {
            this.Log().Debug($"Closing a file: {file}");

            int fileIndex = this.Files.TakeWhile(f => f.FileName != file).Count() + 1;
            int currentIndex = this.SelectedItemIndex;

            this.fileViewModelsSource.RemoveKey(file);
            this.closeSubscriptions[file].Dispose();
            this.closeSubscriptions.Remove(file);

            this.SelectedItemIndex = currentIndex == fileIndex ? fileIndex - 1 : currentIndex;

            var preferences = await this.store.GetObject<UserPreferences>(PreferencesKey);

            await this.AddFileToRecentAsync(preferences, file, true);
            await this.store.InsertObject(PreferencesKey, preferences);

            Locator.CurrentMutable.UnregisterDatabaseServices(file);

            return file;
        }

        private async Task OnShutdownAsync()
        {
            var preferences = await this.store.GetObject<UserPreferences>(PreferencesKey);

            foreach (var file in this.Files)
            {
                await this.AddFileToRecentAsync(preferences, file.FileName, false);
            }

            await this.store.InsertObject(PreferencesKey, preferences);
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
                .SubscribeAsync(name => this.UpdateRecentFileAsync(fileName, name));

            this.closeSubscriptions.Add(fileName, subscriptions);

            this.fileViewModelsSource.AddOrUpdate(fileViewModel);

            this.SelectedItemIndex = this.Files.Count;
        }

        private async Task AddFileToRecentAsync(UserPreferences preferences, string file, bool notifyHomePage)
        {
            var recentFile = preferences.File.RecentFiles.FirstOrDefault(f => f.Path == file);
            RecentFile newRecentFile;

            if (recentFile != null)
            {
                newRecentFile = new RecentFile(recentFile.Name, recentFile.Path, this.scheduler.Now.LocalDateTime);
                preferences.File.RecentFiles.Remove(recentFile);

                if (notifyHomePage)
                {
                    await this.HomePage.RemoveRecentFile.Execute(recentFile);
                }
            } else
            {
                var settings = Locator.Current.GetService<ISettingsService>(file).GetSettings();

                newRecentFile = new RecentFile(settings.ListName, file, this.scheduler.Now.LocalDateTime);
            }

            preferences.File.RecentFiles.Add(newRecentFile);

            if (notifyHomePage)
            {
                await this.HomePage.AddRecentFile.Execute(newRecentFile);
            }
        }

        private async Task UpdateRecentFileAsync(string file, string newName)
        {
            var preferences = await this.store.GetObject<UserPreferences>(PreferencesKey);

            var recentFile = preferences.File.RecentFiles.FirstOrDefault(f => f.Path == file);

            if (recentFile == null)
            {
                return;
            }

            preferences.File.RecentFiles.Remove(recentFile);
            await this.HomePage.RemoveRecentFile.Execute(recentFile);

            var newRecentFile = new RecentFile(newName, file, recentFile.Closed);

            preferences.File.RecentFiles.Add(newRecentFile);
            await this.HomePage.AddRecentFile.Execute(newRecentFile);

            await this.store.InsertObject(PreferencesKey, preferences);
        }
    }
}
