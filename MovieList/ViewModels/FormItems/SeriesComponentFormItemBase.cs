using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using MovieList.Properties;

namespace MovieList.ViewModels.FormItems
{
    public abstract class SeriesComponentFormItemBase : TitledFormItemBase
    {
        private int ordinalNumber;
        private string channel;

        public int OrdinalNumber
        {
            get => this.ordinalNumber;
            set
            {
                this.ordinalNumber = value;
                this.OnPropertyChanged();
            }
        }

        [Required(
            ErrorMessageResourceName = nameof(Messages.ChannelRequired),
            ErrorMessageResourceType = typeof(Messages))]
        public string Channel
        {
            get => this.channel;
            set
            {
                this.channel = value;
                this.OnPropertyChanged();
            }
        }

        public abstract string Title { get; }
        public abstract string Years { get; }

        public IEnumerable<int> AllMonths
            => Enumerable.Range(1, 12);

        public abstract void FullyWriteChanges();
        public abstract void FullyRevertChanges();

        public abstract void OpenForm(SidePanelViewModel sidePanel);
        public abstract void WriteOrdinalNumber();
    }
}
