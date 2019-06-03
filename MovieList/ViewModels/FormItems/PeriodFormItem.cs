using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using MovieList.Data.Models;
using MovieList.Properties;
using MovieList.Validation;

namespace MovieList.ViewModels.FormItems
{
    public class PeriodFormItem : FormItemBase
    {
        private int startMonth;
        private string startYear;
        private int endMonth;
        private string endYear;
        private bool isSingleDayRelease;
        private string numberOfEpisodes;
        private string? posterUrl;

        public PeriodFormItem(Period period)
        {
            this.Period = period;
            this.CopyPeriodProperties();
            this.IsInitialized = true;
        }

        public Period Period { get; }

        public int StartMonth
        {
            get => this.startMonth;
            set
            {
                this.startMonth = value;
                this.OnPropertyChanged();
            }
        }

        [StringRange(
            Min = 1950,
            Max = 2100,
            ErrorMessageResourceName = nameof(Messages.InvalidYear),
            ErrorMessageResourceType = typeof(Messages))]
        public string StartYear
        {
            get => this.startYear;
            set
            {
                this.startYear = value;
                this.OnPropertyChanged();
            }
        }

        public int EndMonth
        {
            get => this.endMonth;
            set
            {
                this.endMonth = value;
                this.OnPropertyChanged();
            }
        }

        [StringRange(
            Min = 1950,
            Max = 2100,
            ErrorMessageResourceName = nameof(Messages.InvalidYear),
            ErrorMessageResourceType = typeof(Messages))]
        public string EndYear
        {
            get => this.endYear;
            set
            {
                this.endYear = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsSingleDayRelease
        {
            get => this.isSingleDayRelease;
            set
            {
                this.isSingleDayRelease = value;
                this.OnPropertyChanged();
            }
        }

        [StringRange(
            Min = 1950,
            Max = 2100,
            ErrorMessageResourceName = nameof(Messages.InvalidNumberOfEpisodes),
            ErrorMessageResourceType = typeof(Messages))]
        public string NumberOfEpisodes
        {
            get => this.numberOfEpisodes;
            set
            {
                this.numberOfEpisodes = value;
                this.OnPropertyChanged();
            }
        }

        [Url(
            ErrorMessageResourceName = nameof(Messages.InvalidPosterUrl),
            ErrorMessageResourceType = typeof(Messages))]
        public string? PosterUrl
        {
            get => this.posterUrl;
            set
            {
                this.posterUrl = value;
                this.OnPropertyChanged();
            }
        }

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.StartMonth, () => this.Period.StartMonth),
                (() => this.StartYear, () => this.Period.StartYear),
                (() => this.EndMonth, () => this.Period.EndMonth),
                (() => this.EndYear, () => this.Period.EndYear),
                (() => this.IsSingleDayRelease, () => this.Period.IsSingleDayRelease),
                (() => this.NumberOfEpisodes, () => this.Period.NumberOfEpisodes),
                (() => this.PosterUrl, () => this.Period.PosterUrl)
            };

        public override void WriteChanges()
        {
            this.Period.StartMonth = this.StartMonth;
            this.Period.StartYear = Int32.Parse(this.StartYear);
            this.Period.EndMonth = this.EndMonth;
            this.Period.EndYear = Int32.Parse(this.EndYear);
            this.Period.IsSingleDayRelease = this.IsSingleDayRelease;
            this.Period.NumberOfEpisodes = Int32.Parse(this.NumberOfEpisodes);
            this.Period.PosterUrl = this.PosterUrl;
            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopyPeriodProperties();
            this.AreChangesPresent = false;
        }

        private void CopyPeriodProperties()
        {
            this.StartMonth = this.Period.StartMonth;
            this.StartYear = this.Period.StartYear.ToString();
            this.EndMonth = this.Period.EndMonth;
            this.EndYear = this.Period.EndYear.ToString();
            this.IsSingleDayRelease = this.Period.IsSingleDayRelease;
            this.NumberOfEpisodes = this.Period.NumberOfEpisodes.ToString();
            this.PosterUrl = this.Period.PosterUrl;
        }
    }
}
