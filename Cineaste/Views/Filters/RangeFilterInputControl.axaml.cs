using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.ReactiveUI;

using Cineaste.Core.ViewModels.Filters;
using Cineaste.Properties;

using ReactiveUI;

namespace Cineaste.Views.Filters
{
    public partial class RangeFilterInputControl : ReactiveUserControl<RangeFilterInputViewModel>
    {
        public RangeFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.Start, v => v.RangeStart.Value)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.End, v => v.RangeEnd.Value)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel!.Description)
                    .Select(description => Messages.ResourceManager.GetString(
                        $"FilterDescription{description}", CultureInfo.CurrentCulture))
                    .WhereNotNull()
                    .Subscribe(description =>
                    {
                        this.CaptionStartTextBlock.Text = String.Format(
                            CultureInfo.CurrentCulture, Messages.FilterRangeStartDescriptionFormat, description);

                        this.CaptionEndTextBlock.Text = String.Format(
                            CultureInfo.CurrentCulture, Messages.FilterRangeEndDescriptionFormat, description);
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
