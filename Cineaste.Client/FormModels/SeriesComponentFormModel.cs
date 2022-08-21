namespace Cineaste.Client.FormModels;

public abstract class SeriesComponentFormModel<TModel> : TitledFormModel<TModel>, ISeriesComponentFormModel
    where TModel : ISeriesComponentModel
{
    public string Title =>
        this.Titles.FirstOrDefault() ?? String.Empty;

    public string Channel { get; set; } = String.Empty;

    public abstract string Years { get; }

    public int SequenceNumber { get; set; }

    public bool IsFirst =>
        this.SequenceNumber == 1;

    public bool IsLast =>
        this.SequenceNumber == this.NextSequenceNumber() - 1;

    protected Func<int> NextSequenceNumber { get; }

    public SeriesComponentFormModel(Func<int> nextSequenceNumber) =>
        this.NextSequenceNumber = nextSequenceNumber;
}
