using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using DynamicData;

using MovieList.Data;
using MovieList.Data.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels
{
    public sealed class MainViewModel : ReactiveObject, IEnableLogger
    {
        private readonly SourceCache<FileViewModel, string> fileViewModelsSource;

        private readonly Dictionary<string, IDisposable> closeSubscriptions = new Dictionary<string, IDisposable>();

        public MainViewModel()
        {
            this.HomePage = new HomePageViewModel();

            this.fileViewModelsSource = new SourceCache<FileViewModel, string>(x => x.FileName);

            this.fileViewModelsSource.Connect()
                .Bind(out var fileViewModels)
                .DisposeMany()
                .Subscribe();

            this.Files = fileViewModels;

            Observable.Return(new List<ReactiveObject> { this.HomePage })
                .Concat(this.fileViewModelsSource.Connect()
                    .ToCollection()
                    .Select(vms => vms.Cast<ReactiveObject>().Prepend(this.HomePage)))
                .ToPropertyEx(this, vm => vm.AllChildren);

            this.OpenFile = ReactiveCommand.CreateFromTask<OpenFileModel, OpenFileModel?>(this.OnOpenFileAsync);
            this.CloseFile = ReactiveCommand.CreateFromTask<string, string>(this.OnCloseFile);

            this.HomePage.OpenFile
                .Merge(this.HomePage.CreateFile)
                .WhereNotNull()
                .Select(file => new OpenFileModel(file))
                .InvokeCommand(this.OpenFile);
        }

        public HomePageViewModel HomePage { get; set; }
        public ReadOnlyObservableCollection<FileViewModel> Files { get; }

        public IEnumerable<ReactiveObject> AllChildren { [ObservableAsProperty] get; } = null!;

        [Reactive]
        public int SelectedItemIndex { get; set; }

        public ReactiveCommand<OpenFileModel, OpenFileModel?> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }

        private async Task<OpenFileModel?> OnOpenFileAsync(OpenFileModel model)
        {
            int fileIndex = this.Files.TakeWhile(file => file.FileName != model.File).Count();

            if (fileIndex != this.Files.Count)
            {
                this.Log().Debug($"The file is already opened: {model.File}. Opening its tab.");
                this.SelectedItemIndex = fileIndex + 1;
                return model;
            }

            this.Log().Debug($"Opening a file: {model.File}");
            Locator.CurrentMutable.RegisterDatabaseServices(model.File);

            bool isFileValid = await Locator.Current.GetService<IDatabaseService>(model.File)
                .ValidateDatabaseAsync();

            if (!isFileValid)
            {
                this.Log().Debug($"Cancelling opening a file: {model.File}");
                Locator.CurrentMutable.UnregisterDatabaseServices(model.File);
                return null;
            }

            var settings = await Locator.Current.GetService<ISettingsService>(model.File)
                .GetSettingsAsync();

            var fileViewModel = new FileViewModel(model.File, settings[SettingsListNameKey]);

            var subscription = fileViewModel.Header.Close.InvokeCommand(this.CloseFile);
            this.closeSubscriptions.Add(model.File, subscription);

            this.fileViewModelsSource.AddOrUpdate(fileViewModel);

            this.SelectedItemIndex = this.AllChildren.Count() - 1;

            return model;
        }

        public async Task<string> OnCloseFile(string file)
        {
            this.Log().Debug($"Closing a file: {file}");

            int fileIndex = this.Files.TakeWhile(f => f.FileName != file).Count() + 1;
            int currentIndex = this.SelectedItemIndex;

            this.fileViewModelsSource.RemoveKey(file);
            this.closeSubscriptions[file].Dispose();
            this.closeSubscriptions.Remove(file);

            await Task.Delay(100);

            this.SelectedItemIndex = currentIndex == fileIndex ? fileIndex - 1 : currentIndex;

            Locator.CurrentMutable.UnregisterDatabaseServices(file);

            return file;
        }
    }
}
