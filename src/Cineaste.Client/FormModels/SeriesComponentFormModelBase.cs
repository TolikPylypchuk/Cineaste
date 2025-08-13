namespace Cineaste.Client.FormModels;

public abstract class SeriesComponentFormModelBase<TRequest, TModel>
    : TitledFormModelBase<TRequest, TModel>, ISeriesComponentFormModel
    where TRequest : IValidatable<TRequest>, ITitledRequest
    where TModel : ISeriesComponentModel
{
    private readonly Func<int> lastSequenceNumber;
    private readonly Func<bool> canSetPoster;

    public string Title =>
        this.Titles.FirstOrDefault() ?? String.Empty;

    public string Channel { get; set; } = String.Empty;

    public abstract string Years { get; }

    public int SequenceNumber { get; set; }

    public bool IsFirst =>
        this.SequenceNumber == FirstSequenceNumber;

    public bool IsLast =>
        this.SequenceNumber == this.lastSequenceNumber();

    public bool CanSetPoster => this.canSetPoster();

    protected Func<int> GetSequenceNumber { get; }

    public SeriesComponentFormModelBase(
        Func<ISeriesComponentFormModel, int> getSequenceNumber,
        Func<int> lastSequenceNumber,
        Func<bool> canSetPoster)
    {
        this.GetSequenceNumber = () => getSequenceNumber(this);
        this.lastSequenceNumber = lastSequenceNumber;
        this.canSetPoster = canSetPoster;
    }
}
