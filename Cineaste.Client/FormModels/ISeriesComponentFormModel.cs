namespace Cineaste.Client.FormModels;

using System.ComponentModel;

public interface ISeriesComponentFormModel : INotifyPropertyChanged
{
    public string Title { get; }

    public string Channel { get; set; }

    public string Years { get; }

    public int SequenceNumber { get; set; }

    public void UpdateRequest();
}
