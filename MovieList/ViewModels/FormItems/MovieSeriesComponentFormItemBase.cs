namespace MovieList.ViewModels.FormItems
{
    public abstract class MovieSeriesComponentFormItemBase : TitledFormItemBase
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
        public abstract string Years { get; }

        public abstract void OpenForm(SidePanelViewModel sidePanel);
    }
}
