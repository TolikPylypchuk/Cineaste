namespace Cineaste.Client.FormModels;

public abstract class SeriesComponentFormModel<TModel> : TitledFormModel<TModel>, ISeriesComponentFormModel
    where TModel : ISeriesComponentModel
{
    private readonly Func<int> lastSequenceNumber;

    public string Title =>
        this.Titles.FirstOrDefault() ?? String.Empty;

    public string Channel { get; set; } = String.Empty;

    public abstract string Years { get; }

    public int SequenceNumber { get; set; }

    public bool IsFirst =>
        this.SequenceNumber == 1;

    public bool IsLast =>
        this.SequenceNumber == this.lastSequenceNumber();

    protected Func<int> GetSequenceNumber { get; }

    public SeriesComponentFormModel(
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber)
    {
        this.GetSequenceNumber = () => getSequenceNumber(this);
        this.lastSequenceNumber = lastSequenceNumber;
    }
}
