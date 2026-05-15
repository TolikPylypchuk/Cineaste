using System.ComponentModel;

namespace Cineaste.Client.FormModels;

public sealed class SeasonPartFormModel : IdentifyableFormModelBase<SeasonPartRequest, SeasonPartModel>
{
    public ReleasePeriodFormModel Period { get; private set; }

    public string RottenTomatoesId { get; set; }

    public SeasonPartFormModel()
        : this(DateOnly.FromDateTime(DateTime.Now))
    { }

    public SeasonPartFormModel(DateOnly date)
    {
        this.Period = new(date);
        this.Period.PropertyChanged += this.OnPeriodUpdated;
        this.RottenTomatoesId = String.Empty;

        this.FinishInitialization();
    }

    public override SeasonPartRequest CreateRequest() =>
        new(this.BackingModel?.Id, this.Period.CreateRequest(), this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var part = this.BackingModel;

        this.Period.CopyFrom(part?.Period);
        this.RottenTomatoesId = part?.RottenTomatoesId ?? String.Empty;
    }

    private void OnPeriodUpdated(object? sender, PropertyChangedEventArgs e) =>
        this.UpdateRequest();
}
