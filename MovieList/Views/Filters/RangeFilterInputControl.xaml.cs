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
    public abstract class RangeFilterInputControlBase : ReactiveUserControl<RangeFilterInputViewModel> { }

    public partial class RangeFilterInputControl : RangeFilterInputControlBase
    {
        public RangeFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.RangeStartBox.DisposeWith(disposables);
                this.RangeEndBox.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Start, v => v.RangeStartBox.Number)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.End, v => v.RangeEndBox.Number)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Description)
                    .Select(description => Messages.ResourceManager.GetString($"FilterDescription{description}"))
                    .WhereNotNull()
                    .Subscribe(description =>
                    {
                        HintAssist.SetHint(
                            this.RangeStartBox,
                            String.Format(Messages.FilterRangeStartDescriptionFormat, description));

                        HintAssist.SetHint(
                            this.RangeEndBox,
                            String.Format(Messages.FilterRangeEndDescriptionFormat, description));
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
