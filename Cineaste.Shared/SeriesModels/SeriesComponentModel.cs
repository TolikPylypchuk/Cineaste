namespace Cineaste.Shared.SeriesModels;

using System.Text.Json.Serialization;

public abstract record SeriesComponentModel(int SequenceNumber)
{
    [JsonIgnore]
    public abstract string Title { get; }

    [JsonIgnore]
    public abstract string Years { get; }
}
