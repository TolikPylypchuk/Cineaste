using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

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
        }

        public ReadOnlyObservableCollection<RecentFile> RecentFiles
            => this.recentFiles;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public bool ShowRecentFiles { [ObservableAsProperty] get; }
    }
}
