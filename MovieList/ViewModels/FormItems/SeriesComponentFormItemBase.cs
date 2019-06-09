using System.Collections.Generic;
using System.Linq;

namespace MovieList.ViewModels.FormItems
{
    public abstract class SeriesComponentFormItemBase : TitledFormItemBase
    {
        private int ordinalNumber;

        public int OrdinalNumber
        {
            get => this.ordinalNumber;
            set
            {
                this.ordinalNumber = value;
                this.OnPropertyChanged();
            }
        }

        public abstract string Title { get; }

        public IEnumerable<int> AllMonths
            => Enumerable.Range(1, 12);

        public abstract void OpenForm(SidePanelViewModel sidePanel);
    }
}
