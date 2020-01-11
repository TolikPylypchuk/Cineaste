using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using System.Threading.Tasks;

using MovieList.Data.Models;
using MovieList.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class PeriodFormViewModel : FormBase<Period, PeriodFormViewModel>
    {
        public PeriodFormViewModel(
            Period period,
            IObservable<bool> canDelete,
            ResourceManager? resourceManager,
            IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Period = period;
            this.CopyProperties();

            this.StartYearRule = this.ValidationRule(vm => vm.StartYear, SeriesMinYear, SeriesMaxYear, "Year");
            this.EndYearRule = this.ValidationRule(vm => vm.EndYear, SeriesMinYear, SeriesMaxYear, "Year");

            this.NumberOfEpisodesRule = this.ValidationRule(
                vm => vm.NumberOfEpisodes,
                PeriodMinNumberOfEpisodes,
                PeriodMaxNumberOfEpisodes,
                nameof(this.NumberOfEpisodes));

            this.PosterUrlRule = this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

            var periodValid = this.WhenAnyValue(
                    vm => vm.StartMonth,
                    vm => vm.StartYear,
                    vm => vm.EndMonth,
                    vm => vm.EndYear)
                .Throttle(TimeSpan.FromMilliseconds(250), this.Scheduler)
                .Select(((int StartMonth, string StartYear, int EndMonth, string EndYear) values) =>
                    Int32.TryParse(values.StartYear, out int startYear) &&
                    Int32.TryParse(values.EndYear, out int endYear) &&
                    (startYear < endYear || startYear == endYear && values.StartMonth <= values.EndMonth))
                .ObserveOn(RxApp.MainThreadScheduler);

            this.PeriodRule = this.ValidationRule(
                _ => periodValid.StartWith(true),
                (vm, isValid) => isValid ? String.Empty : vm.ResourceManager.GetString("ValidationPeriodInvalid"));

            this.CanDeleteWhen(canDelete);

            this.InitializeValueDependencies();

            this.EnableChangeTracking();
        }

        public Period Period { get; }

        [Reactive]
        public int StartMonth { get; set; }

        [Reactive]
        public string StartYear { get; set; } = String.Empty;

        [Reactive]
        public int EndMonth { get; set; }

        [Reactive]
        public string EndYear { get; set; } = String.Empty;

        [Reactive]
        public string NumberOfEpisodes { get; set; } = String.Empty;

        [Reactive]
        public bool IsSingleDayRelease { get; set; }

        [Reactive]
        public string PosterUrl { get; set; } = String.Empty;

        [Reactive]
        public bool ShowPosterUrl { get; set; } = true;

        public ValidationHelper StartYearRule { get; }
        public ValidationHelper EndYearRule { get; }
        public ValidationHelper NumberOfEpisodesRule { get; }
        public ValidationHelper PosterUrlRule { get; }
        public ValidationHelper PeriodRule { get; }

        public override bool IsNew
            => this.Period.Id == default;

        protected override PeriodFormViewModel Self
            => this;

        protected override void EnableChangeTracking()
        {
            this.TrackChanges(vm => vm.StartMonth, vm => vm.Period.StartMonth);
            this.TrackChanges(vm => vm.StartYear, vm => vm.Period.StartYear.ToString());
            this.TrackChanges(vm => vm.EndMonth, vm => vm.Period.EndMonth);
            this.TrackChanges(vm => vm.EndYear, vm => vm.Period.EndYear.ToString());
            this.TrackChanges(vm => vm.NumberOfEpisodes, vm => vm.Period.NumberOfEpisodes.ToString());
            this.TrackChanges(vm => vm.IsSingleDayRelease, vm => vm.Period.IsSingleDayRelease);
            this.TrackChanges(vm => vm.PosterUrl, vm => vm.Period.PosterUrl.EmptyIfNull());

            base.EnableChangeTracking();
        }

        protected override Task<Period> OnSaveAsync()
        {
            this.Period.StartMonth = this.StartMonth;
            this.Period.StartYear = Int32.Parse(this.StartYear);
            this.Period.EndMonth = this.EndMonth;
            this.Period.EndYear = Int32.Parse(this.EndYear);
            this.Period.NumberOfEpisodes = Int32.Parse(this.NumberOfEpisodes);
            this.Period.IsSingleDayRelease = this.IsSingleDayRelease;
            this.Period.PosterUrl = this.PosterUrl.NullIfEmpty();

            return Task.FromResult(this.Period);
        }

        [SuppressMessage("ReSharper", "RedundantCast")]
        protected override Task<Period?> OnDeleteAsync()
            => Task.FromResult((Period?)this.Period);

        protected override void CopyProperties()
        {
            this.StartMonth = this.Period.StartMonth;
            this.StartYear = this.Period.StartYear.ToString();
            this.EndMonth = this.Period.EndMonth;
            this.EndYear = this.Period.EndYear.ToString();
            this.NumberOfEpisodes = this.Period.NumberOfEpisodes.ToString();
            this.IsSingleDayRelease = this.Period.IsSingleDayRelease;
            this.PosterUrl = this.Period.PosterUrl.EmptyIfNull();
        }

        private void InitializeValueDependencies()
        {
            this.WhenAnyValue(
                    vm => vm.StartMonth,
                    vm => vm.IsSingleDayRelease,
                    (month, isSingleDayRelease) => (Month: month, IsSingleDayRelease: isSingleDayRelease))
                .Where(value => value.IsSingleDayRelease)
                .Select(value => value.Month)
                .BindTo(this, vm => vm.EndMonth);

            this.WhenAnyValue(
                    vm => vm.StartYear,
                    vm => vm.IsSingleDayRelease,
                    (year, isSingleDayRelease) => (Year: year, IsSingleDayRelease: isSingleDayRelease))
                .Where(value => value.IsSingleDayRelease)
                .Select(value => value.Year)
                .BindTo(this, vm => vm.EndYear);
        }
    }
}
