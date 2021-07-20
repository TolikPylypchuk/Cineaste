using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels;

using ReactiveUI;

namespace Cineaste.Views
{
    public partial class FileControl : ReactiveUserControl<FileViewModel>
    {
        public FileControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Content)
                    .WhereNotNull()
                    .DistinctUntilChanged()
                    .Subscribe(content =>
                    {
                        var selectedItem = content switch
                        {
                            FileMainContentViewModel => this.ListItem,
                            _ => this.SettingsItem
                        };

                        if (selectedItem.Content is ViewModelViewHost host)
                        {
                            host.ViewModel = content;
                        }

                        this.Sidebar.SelectedItem = selectedItem;
                    })
                    .DisposeWith(disposables);

                var isInitialized = this.ViewModel!.IsInitialized
                    .Where(isInitialized => isInitialized);

                this.Sidebar.GetObservable(SelectingItemsControl.SelectedItemProperty)
                    .SkipUntil(isInitialized)
                    .WhereNotNull()
                    .OfType<TabItem>()
                    .Select(item => item == this.ListItem
                        ? this.ViewModel!.SwitchToList
                        : this.ViewModel!.SwitchToSettings)
                    .SubscribeAsync(command => command.Execute())
                    .DisposeWith(disposables);

                isInitialized.Select(_ => this.ListItem)
                    .Take(1)
                    .BindTo(this, v => v.Sidebar.SelectedItem)
                    .DisposeWith(disposables);
            });
        }
    }
}
