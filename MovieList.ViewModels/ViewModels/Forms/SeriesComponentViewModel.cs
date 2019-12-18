using System;
using System.Collections.Generic;
using System.Linq;
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
                .ToCollection()
                .Select(periods => this.GetYears(periods.ToList()))
                .BindTo(this, vm => vm.Years);

            form.WhenAnyValue(vm => vm.SequenceNumber)
                .BindTo(this, vm => vm.SequenceNumber);

            this.Select = ReactiveCommand.Create<Unit, ReactiveObject>(_ => this.Form);

            this.MoveUp = ReactiveCommand.CreateFromObservable(
                () => form.MoveUp.Execute(), form.MoveUp.CanExecute);

            this.MoveDown = ReactiveCommand.CreateFromObservable(
                () => form.MoveDown.Execute(), form.MoveDown.CanExecute);
        }

        public ReactiveObject Form { get; }

        [Reactive]
        public string Title { get; set; } = String.Empty;

        [Reactive]
        public string Years { get; set; } = String.Empty;

        [Reactive]
        public int SequenceNumber { get; set; }

        public ReactiveCommand<Unit, ReactiveObject> Select { get; }
        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        private string GetYears(IList<PeriodFormViewModel> periods)
        {
            if (periods.Count == 0)
            {
                return String.Empty;
            }

            var first = periods[0];
            var last = periods[^1];

            return first.StartYear == last.EndYear ? first.StartYear : $"{first.StartYear}-{last.EndYear}";
        }
    }
}
