using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

using HandyControl.Data;

using MovieList.Commands;
using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.ViewModels
{
    public class KindViewModel : ViewModelBase
    {
        public KindViewModel(Kind kind)
        {
            this.Kind = kind;

            this.ChangeColorForMovie = new DelegateCommand<FrameworkElement>(element => Util.OpenColorPickerPopup(
                element, this.ColorForMovie, color => this.ColorForMovie = color.ToString()));

            this.ChangeColorForSeries = new DelegateCommand<FrameworkElement>(element => Util.OpenColorPickerPopup(
                element as FrameworkElement, this.ColorForSeries, color => this.ColorForSeries = color.ToString()));
        }

        public DelegateCommand<FrameworkElement> ChangeColorForMovie { get; }
        public DelegateCommand<FrameworkElement> ChangeColorForSeries { get; }

        public Kind Kind { get; }

        [Required(
            ErrorMessageResourceName = nameof(Messages.NameRequired),
            ErrorMessageResourceType = typeof(Messages))]
        public string Name
        {
            get => this.Kind.Name;
            set
            {
                this.Kind.Name = value;
                this.OnPropertyChanged();
            }
        }

        public string ColorForMovie
        {
            get => this.Kind.ColorForMovie;
            set
            {
                this.Kind.ColorForMovie = value;
                this.OnPropertyChanged();
            }
        }

        public string ColorForSeries
        {
            get => this.Kind.ColorForSeries;
            set
            {
                this.Kind.ColorForSeries = value;
                this.OnPropertyChanged();
            }
        }

        public Func<string, OperationResult<bool>> VerifyName
            => this.Verify(nameof(Name));
    }
}
