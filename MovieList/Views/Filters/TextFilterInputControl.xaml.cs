using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using MaterialDesignThemes.Wpf;

using MovieList.Core;
using MovieList.Core.ViewModels.Filters;
using MovieList.Properties;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class TextFilterInputControlBase : ReactiveUserControl<TextFilterInputViewModel> { }

    public partial class TextFilterInputControl : TextFilterInputControlBase
    {
        public TextFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Text, v => v.InputBox.Text)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Description)
                    .Select(description => Messages.ResourceManager.GetString($"FilterDescription{description}"))
                    .WhereNotNull()
                    .Subscribe(description => HintAssist.SetHint(this.InputBox, description))
                    .DisposeWith(disposables);
            });
        }
    }
}
