namespace Cineaste.Core.Domain;

public sealed record Title
{
    private int sequenceNumber;

    public string Name { get; }

    public int SequenceNumber
    {
        get => sequenceNumber;
        set => sequenceNumber = Require.Positive(value);
    }

    public bool IsOriginal { get; }

    public Title(string name, int sequenceNumber, bool isOriginal)
    {
        this.Name = Require.NotBlank(name);
        this.SequenceNumber = sequenceNumber;
        this.IsOriginal = isOriginal;
    }
}
