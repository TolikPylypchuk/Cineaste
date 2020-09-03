namespace MovieList.Core.ViewModels.Forms.Base
{
    public interface ISeriesComponentForm : IReactiveForm
    {
        SeriesFormViewModel Parent { get; }

        string Channel { get; set; }
        int SequenceNumber { get; set; }

        int GetNextYear();
    }
}
