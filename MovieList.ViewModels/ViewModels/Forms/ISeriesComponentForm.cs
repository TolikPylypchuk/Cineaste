using ReactiveUI;

namespace MovieList.ViewModels.Forms
{
    public interface ISeriesComponentForm : IReactiveObject
    {
        SeriesFormViewModel Parent { get; }

        string Channel { get; set; }
        int SequenceNumber { get; set; }

        int GetNextYear();
    }
}
