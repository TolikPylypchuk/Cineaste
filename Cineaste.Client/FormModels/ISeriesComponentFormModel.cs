namespace Cineaste.Client.FormModels;

public interface ISeriesComponentFormModel
{
    public string Title { get; }

    public string Years { get; }

    public int SequenceNumber { get; set; }
}
