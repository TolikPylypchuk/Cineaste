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

        public abstract void OpenForm(SidePanelViewModel sidePanel);
    }
}
