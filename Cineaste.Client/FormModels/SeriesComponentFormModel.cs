namespace Cineaste.Client.FormModels;

public abstract class SeriesComponentFormModel<TModel, TRequest>
    : TitledFormModel<TModel, TRequest>, ISeriesComponentFormModel
    where TModel : ISeriesComponentModel
{
    public string Title =>
        this.Titles.FirstOrDefault() ?? String.Empty;

    public abstract string Years { get; }

    public int SequenceNumber { get; set; }
}
