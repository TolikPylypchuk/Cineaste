using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using MovieList.Properties;
using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class FileControlBase : ReactiveUserControl<FileViewModel> { }

    public partial class FileControl : FileControlBase
    {
        public FileControl()
        {
            this.InitializeComponent();

            this.Events().Loaded
                .Subscribe(e => this.Navigation.WillSelectNavigationItemCallbackAsync =
                    this.OnWillSelectNavigationItemAsync);

            this.Events().Unloaded
                .Subscribe(e => this.Navigation.WillSelectNavigationItemCallbackAsync = null);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.InitializeSideNavigation(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Content, v => v.ContentHost.ViewModel)
                    .DisposeWith(disposables);
            });
        }

        public INavigationItem ListItem { get; private set; } = null!;
        public INavigationItem StatsItem { get; private set; } = null!;
        public INavigationItem SettingsItem { get; private set; } = null!;

        private void InitializeSideNavigation(CompositeDisposable disposables)
        {
            this.ListItem = new FirstLevelNavigationItem
            {
                Label = Messages.NavigationList,
                Icon = PackIconKind.FormatListBulleted,
                IsSelectable = true
            };

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Content,
                    v => v.ListItem.IsSelected,
                    content => content == this.ViewModel.List)
                .DisposeWith(disposables);

            this.StatsItem = new FirstLevelNavigationItem
            {
                Label = Messages.NavigationStats,
                Icon = PackIconKind.ChartLine,
                IsSelectable = false
            };

            this.SettingsItem = new FirstLevelNavigationItem
            {
                Label = Messages.NavigationSettings,
                Icon = PackIconKind.Cogs,
                IsSelectable = true
            };

            this.OneWayBind(
                    this.ViewModel,
                    vm => vm.Content,
                    v => v.SettingsItem.IsSelected,
                    content => content == this.ViewModel.Settings)
                .DisposeWith(disposables);

            this.Navigation.Items = new List<INavigationItem>
            {
                this.ListItem,
                this.StatsItem,
                this.SettingsItem
            };
        }

        private async Task<bool> OnWillSelectNavigationItemAsync(
            INavigationItem currentItem,
            INavigationItem selectedItem)
        {
            if (selectedItem == this.ListItem)
            {
                await this.ViewModel.SwitchToList.Execute();
            } else if (selectedItem == this.StatsItem)
            {
                await this.ViewModel.SwitchToStats.Execute();
            } else if (selectedItem == this.SettingsItem)
            {
                await this.ViewModel.SwitchToSettings.Execute();
            }

            return true;
        }
    }
}
