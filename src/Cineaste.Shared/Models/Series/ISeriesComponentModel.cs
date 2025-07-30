namespace Cineaste.Shared.Models.Series;

public interface ISeriesComponentModel : IIdentifyableModel, ITitledModel
{
    public int SequenceNumber { get; }
}
