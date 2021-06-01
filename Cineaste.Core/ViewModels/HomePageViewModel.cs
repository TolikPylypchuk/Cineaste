using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Akavache;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core.DialogModels;
using MovieList.Core.Models;
using MovieList.Core.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Core.Constants;
using static MovieList.Core.ServiceUtil;

namespace MovieList.Core.ViewModels
{
    public sealed class HomePageViewModel : ReactiveObject
    {
        private readonly IBlobCache store;
        private readonly ReadOnlyObservableCollection<RecentFileViewModel> recentFiles;
        private readonly SourceCache<RecentFileViewModel, string> recentFilesSource = new(vm => vm.File.Path);

        public HomePageViewModel(IObservable<bool> showRecentFiles, IBlobCache? store = null)
        {
            this.store = store ?? GetDefaultService<IBlobCache>(StoreKey);

            this.recentFilesSource.Connect()
                .Sort(SortExpressionComparer<RecentFileViewModel>.Descending(vm => vm.File.Closed))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.recentFiles)
                .DisposeMany()
                .Subscribe();

            var preferencesObservable = this.store.GetObject<UserPreferences?>(PreferencesKey).WhereNotNull();

            preferencesObservable
                .SelectMany(preferences => preferences.File.RecentFiles)
                .Select(file => new RecentFileViewModel(file, this))
                .Subscribe(recentFilesSource.AddOrUpdate);

            preferencesObservable
                .Select(preferences => preferences.File.ShowRecentFiles)
                .Merge(showRecentFiles)
                .ToPropertyEx(this, vm => vm.ShowRecentFiles, initialValue: false);

            this.WhenAnyValue(vm => vm.RecentFiles.Count)
                .Select(count => count != 0)
                .ToPropertyEx(this, vm => vm.RecentFilesPresent);

            this.CreateFile = ReactiveCommand.CreateFromObservable(this.OnCreateFile);
            this.OpenFile = ReactiveCommand.CreateFromObservable<string?, string?>(this.OnOpenFile);
            this.OpenRecentFile = ReactiveCommand.CreateFromObservable<string, string?>(this.OnOpenRecentFile);

            var canRemoveSelectedRecentFiles = this.recentFilesSource.Connect()
                .AutoRefresh(file => file.IsSelected)
                .ToCollection()
                .Select(files => files.Any(file => file.IsSelected));

            this.RemoveSelectedRecentFiles = ReactiveCommand.CreateFromObservable(
                this.OnRemoveSelectedRecentFiles, canRemoveSelectedRecentFiles);

            this.AddRecentFile = ReactiveCommand.Create<RecentFile>(
                file => this.recentFilesSource.AddOrUpdate(new RecentFileViewModel(file, this)));

            this.RemoveRecentFile = ReactiveCommand.Create<RecentFile>(
                file => this.recentFilesSource.RemoveKey(file.Path));

            this.OpenRecentFile
                .WhereNotNull()
                .InvokeCommand(this.OpenFile);
        }

        public ReadOnlyObservableCollection<RecentFileViewModel> RecentFiles =>
            this.recentFiles;

        public bool ShowRecentFiles { [ObservableAsProperty] get; }
        public bool RecentFilesPresent { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, CreateFileModel?> CreateFile { get; }
        public ReactiveCommand<string?, string?> OpenFile { get; }
        public ReactiveCommand<string, string?> OpenRecentFile { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelectedRecentFiles { get; }

        public ReactiveCommand<RecentFile, Unit> AddRecentFile { get; }
        public ReactiveCommand<RecentFile, Unit> RemoveRecentFile { get; }

        private IObservable<CreateFileModel?> OnCreateFile() =>
            Dialog.SaveFile.Handle(String.Empty)
                .Do(_ => this.Log().Debug("Creating a new list"))
                .SelectNotNull(fileName => new CreateFileModel(fileName, Path.GetFileNameWithoutExtension(fileName)));

        private IObservable<string?> OnOpenFile(string? fileName)
        {
            this.Log().Debug(fileName is null ? "Opening a list" : $"Opening a list: {fileName}");
            return fileName != null ? Observable.Return(fileName) : Dialog.OpenFile.Handle(Unit.Default);
        }

        private IObservable<string?> OnOpenRecentFile(string fileName) =>
            File.Exists(fileName)
                ? Observable.Return(fileName)
                : Dialog.Confirm.Handle(new ConfirmationModel("RemoveRecentFile"))
                    .SelectMany(shouldRemoveFile => shouldRemoveFile
                        ? this.RemoveRecentFileEntry(fileName)
                        : Observable.Return(Unit.Default))
                    .Select(_ => (string?)null);

        private IObservable<Unit> RemoveRecentFileEntry(string fileName) =>
            this.store.GetObject<UserPreferences?>(PreferencesKey)
                .WhereNotNull()
                .Eager()
                .Do(_ => this.Log().Debug($"Removing recent file: {fileName}"))
                .Do(_ => this.recentFilesSource.Remove(fileName))
                .Do(preferences => preferences.File.RecentFiles.RemoveAll(file => file.Path == fileName))
                .SelectMany(preferences => this.store.InsertObject(PreferencesKey, preferences).Eager());

        private IObservable<Unit> OnRemoveSelectedRecentFiles() =>
            this.store.GetObject<UserPreferences?>(PreferencesKey)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(this.RemoveRecentFiles)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SelectMany(preferences => this.store.InsertObject(PreferencesKey, preferences))
                .Eager();

        private void RemoveRecentFiles(UserPreferences preferences)
        {
            var filesToRemove = this.recentFiles
                .Where(file => file.IsSelected)
                .ToList();

            string fileNames = filesToRemove
                .Select(file => file.File.Name)
                .Aggregate((acc, file) => $"{acc}, {file}");

            this.Log().Debug($"Removing recent files: {fileNames}");

            this.recentFilesSource.Remove(filesToRemove);
            preferences.File.RecentFiles.RemoveMany(filesToRemove.Select(file => file.File));
        }
    }
}
