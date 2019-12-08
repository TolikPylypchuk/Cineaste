using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace MovieList.ViewModels.Forms
{
    public sealed class SeriesComponentViewModel : ReactiveObject, IEnableLogger
    {
        public SeriesComponentViewModel(SeasonFormViewModel form)
        {
            this.Log().Debug($"Creating a view model for a season inside the series form: {form.Season}");

            this.Form = form;

            form.FormTitle.BindTo(this, vm => vm.Title);

            form.Periods.ToObservableChangeSet()
                .AutoRefresh(period => period.StartYear)
                .AutoRefresh(period => period.EndYear)
                .Select(_ => this.GetYears(form.Periods))
                .BindTo(this, vm => vm.Years);

            this.Select = ReactiveCommand.Create<Unit, ReactiveObject>(_ => this.Form);
        }

        public ReactiveObject Form { get; }

        [Reactive]
        public string Title { get; set; } = String.Empty;

        [Reactive]
        public string Years { get; set; } = String.Empty;

        public ReactiveCommand<Unit, ReactiveObject> Select { get; }

        private string GetYears(IList<PeriodFormViewModel> periods)
        {
            var first = periods[0];
            var last = periods[^1];

            return first.StartYear == last.EndYear ? first.StartYear : $"{first.StartYear}-{last.EndYear}";
        }
    }
}
