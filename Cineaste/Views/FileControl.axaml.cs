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
                    .Subscribe(content =>
                    {
                        foreach (TabItem item in this.Sidebar.Items)
                        {
                            item.Content = null;
                        }

                        var selectedItem = content switch
                        {
                            FileMainContentViewModel => this.ListItem,
                            _ => this.SettingsItem
                        };

                        selectedItem.Content = content;
                        this.Sidebar.SelectedItem = selectedItem;
                    })
                    .DisposeWith(disposables);

                this.Sidebar.GetObservable(SelectingItemsControl.SelectedItemProperty)
                    .WhereNotNull()
                    .OfType<TabItem>()
                    .Select(item => item == this.ListItem
                        ? this.ViewModel!.SwitchToList
                        : this.ViewModel!.SwitchToSettings)
                    .SubscribeAsync(command => command.Execute())
                    .DisposeWith(disposables);
            });
        }
    }
}
