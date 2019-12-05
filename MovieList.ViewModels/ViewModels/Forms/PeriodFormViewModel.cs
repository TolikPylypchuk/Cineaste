using System;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Threading.Tasks;

using MovieList.Data.Models;

using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using static MovieList.Data.Constants;

namespace MovieList.ViewModels.Forms
{
    public sealed class PeriodFormViewModel : FormViewModelBase<Period, PeriodFormViewModel>
    {
        public PeriodFormViewModel(Period period, IObservable<bool> canDelete, ResourceManager? resourceManager)
            : base(resourceManager)
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

            this.CanDeleteWhen(canDelete);

            this.EnableChangeTracking();
        }

        public Period Period { get; }

        [Reactive]
        public int StartMonth { get; set; }

        [Reactive]
        public string StartYear { get; set; } = null!;

        [Reactive]
        public int EndMonth { get; set; }

        [Reactive]
        public string EndYear { get; set; } = null!;

        [Reactive]
        public string NumberOfEpisodes { get; set; } = null!;

        [Reactive]
        public bool IsSingleDayRelease { get; set; }

        [Reactive]
        public string PosterUrl { get; set; } = null!;

        public ValidationHelper StartYearRule { get; }
        public ValidationHelper EndYearRule { get; }
        public ValidationHelper NumberOfEpisodesRule { get; }
        public ValidationHelper PosterUrlRule { get; }

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

            this.TrackValidation(this.StartYearRule);
            this.TrackValidation(this.EndYearRule);
            this.TrackValidation(this.NumberOfEpisodesRule);
            this.TrackValidation(this.PosterUrlRule);

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
    }
}
