using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

using Cineaste.Core;
using Cineaste.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace Cineaste.Views.Forms.Preferences
{
    public partial class TagItemControl : ReactiveUserControl<TagItemViewModel>
    {
        public TagItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Name, v => v.TextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel, vm => vm.Color, v => v.ColorTagBorder.Background)
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                    this.WhenAnyValue(v => v.ViewModel!.Category),
                    this.WhenAnyValue(v => v.ViewModel!.Description),
                    (category, description) => !String.IsNullOrEmpty(description)
                        ? $"{category} | {description}"
                        : category)
                    .Subscribe(toolTip => ToolTip.SetTip(this, toolTip))
                    .DisposeWith(disposables);

                if (this.ViewModel!.CanSelect)
                {
                    this.GetObservable(PointerReleasedEvent)
                        .Discard()
                        .InvokeCommand(this.ViewModel.Select)
                        .DisposeWith(disposables);
                } else
                {
                    this.Cursor = Cursor.Parse(nameof(StandardCursorType.Hand));
                }

                this.DeleteButton.GetObservable(Button.ClickEvent)
                        .Discard()
                        .InvokeCommand(this.ViewModel.Delete)
                        .DisposeWith(disposables);
            });
        }
    }
}
