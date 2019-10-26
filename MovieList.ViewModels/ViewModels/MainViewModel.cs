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
            this.CloseFile = ReactiveCommand.Create<string, string>(this.OnCloseFile);

            this.HomePage.OpenFile
                .Merge(this.HomePage.CreateFile)
                .WhereNotNull()
                .Select(file => new OpenFileModel(file))
                .InvokeCommand(this.OpenFile);
        }

        public HomePageViewModel HomePage { get; set; }
        public ReadOnlyObservableCollection<FileViewModel> Files { get; }

        public IEnumerable<ReactiveObject> AllChildren { [ObservableAsProperty] get; } = null!;

        public ReactiveCommand<OpenFileModel, OpenFileModel?> OpenFile { get; }
        public ReactiveCommand<string, string> CloseFile { get; }

        private async Task<OpenFileModel?> OnOpenFileAsync(OpenFileModel model)
        {
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

            return model;
        }

        public string OnCloseFile(string file)
        {
            this.Log().Debug($"Closing a file: {file}");

            this.fileViewModelsSource.RemoveKey(file);
            this.closeSubscriptions[file].Dispose();
            this.closeSubscriptions.Remove(file);

            Locator.CurrentMutable.UnregisterDatabaseServices(file);

            return file;
        }
    }
}
