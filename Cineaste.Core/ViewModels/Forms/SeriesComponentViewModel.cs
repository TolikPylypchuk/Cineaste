using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Cineaste.Core.ViewModels.Forms.Base;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace Cineaste.Core.ViewModels.Forms
{
    public sealed class SeriesComponentViewModel : ReactiveObject
    {
        private SeriesComponentViewModel(SeasonFormViewModel form)
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

            this.Select = ReactiveCommand.Create<Unit, ISeriesComponentForm>(_ => this.Form);

            this.MoveUp = ReactiveCommand.CreateFromObservable(
                () => form.MoveUp.Execute(), form.MoveUp.CanExecute);

            this.MoveDown = ReactiveCommand.CreateFromObservable(
                () => form.MoveDown.Execute(), form.MoveDown.CanExecute);
        }

        private SeriesComponentViewModel(SpecialEpisodeFormViewModel form)
        {
            this.Log().Debug(
                $"Creating a view model for a special episode inside the series form: {form.SpecialEpisode}");

            this.Form = form;

            form.FormTitle.BindTo(this, vm => vm.Title);

            form.WhenAnyValue(vm => vm.Year)
                .BindTo(this, vm => vm.Years);

            form.WhenAnyValue(vm => vm.SequenceNumber)
                .BindTo(this, vm => vm.SequenceNumber);

            this.Select = ReactiveCommand.Create<Unit, ISeriesComponentForm>(_ => this.Form);

            this.MoveUp = ReactiveCommand.CreateFromObservable(
                () => form.MoveUp.Execute(), form.MoveUp.CanExecute);

            this.MoveDown = ReactiveCommand.CreateFromObservable(
                () => form.MoveDown.Execute(), form.MoveDown.CanExecute);
        }

        public ISeriesComponentForm Form { get; }

        [Reactive]
        public string Title { get; set; } = String.Empty;

        [Reactive]
        public string Years { get; set; } = String.Empty;

        [Reactive]
        public int SequenceNumber { get; set; }

        public ReactiveCommand<Unit, ISeriesComponentForm> Select { get; }
        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        public static SeriesComponentViewModel FromForm(ISeriesComponentForm form) =>
            form switch
            {
                SeasonFormViewModel vm => new SeriesComponentViewModel(vm),
                SpecialEpisodeFormViewModel vm => new SeriesComponentViewModel(vm),
                _ => throw new NotSupportedException(
                    $"Cannot create a series component view model for type {form.GetType()}")
            };

        private string GetYears(IList<PeriodFormViewModel> periods)
        {
            if (periods.Count == 0)
            {
                return String.Empty;
            }

            var first = periods[0];
            var last = periods[^1];

            return first.StartYear == last.EndYear ? first.StartYear.ToString() : $"{first.StartYear}-{last.EndYear}";
        }
    }
}
