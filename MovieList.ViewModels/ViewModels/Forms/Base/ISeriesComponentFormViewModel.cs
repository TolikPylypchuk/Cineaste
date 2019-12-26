namespace MovieList.ViewModels.Forms.Base
{
    public interface ISeriesComponentFormViewModel : IFormViewModel
    {
        SeriesFormViewModel Parent { get; }

        string Channel { get; set; }
        int SequenceNumber { get; set; }

        int GetNextYear();
    }
}
