using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Akavache;

using DynamicData;
using DynamicData.Binding;

using MovieList.Preferences;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using static MovieList.Constants;

namespace MovieList.ViewModels
{
    public sealed class HomePageViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<RecentFile> recentFiles;

        public HomePageViewModel(IBlobCache? store = null)
        {
            store ??= Locator.Current.GetService<IBlobCache>(Store);

            var files = new SourceList<RecentFile>();

            store.GetObject<UserPreferences>(MainPreferences)
                .Select(preferences => preferences.File.RecentFiles)
                .Subscribe(files.AddRange);

            files.Connect()
                .Sort(SortExpressionComparer<RecentFile>.Ascending(file => file.Closed))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this.recentFiles)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(vm => vm.RecentFiles.Count)
                .Select(count => count != 0)
                .ToPropertyEx(this, vm => vm.ShowRecentFiles);

            this.CreateFile = ReactiveCommand.CreateFromTask(this.OnCreateFile);
            this.OpenFile = ReactiveCommand.CreateFromTask<string?, string?>(this.OnOpenFile);
        }

        public ReadOnlyObservableCollection<RecentFile> RecentFiles
            => this.recentFiles;

        public bool ShowRecentFiles { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, string?> CreateFile { get; set; }
        public ReactiveCommand<string?, string?> OpenFile { get; set; }

        private async Task<string?> OnCreateFile()
        {
            this.Log().Debug("Creating a new list.");
            string? listName = await Dialog.CreateList.Handle(Unit.Default);

            return listName is null
                ? null
                : await Dialog.SaveFile.Handle($"{listName}.{ListFileExtension}");
        }

        private async Task<string?> OnOpenFile(string? fileName)
        {
            this.Log().Debug(fileName is null ? "Opening a list." : $"Opening a list: ${fileName}.");
            return fileName ?? await Dialog.OpenFile.Handle(Unit.Default);
        }
    }
}
