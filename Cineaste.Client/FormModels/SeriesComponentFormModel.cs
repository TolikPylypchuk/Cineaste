namespace Cineaste.Client.FormModels;

public abstract class SeriesComponentFormModel<TModel> : TitledFormModel<TModel>, ISeriesComponentFormModel
    where TModel : ISeriesComponentModel
{
    public string Title =>
        this.Titles.FirstOrDefault() ?? String.Empty;

    public string Channel { get; set; } = String.Empty;

    public abstract string Years { get; }

    public int SequenceNumber { get; set; }
}
