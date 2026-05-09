namespace Cineaste.Core.Domain;

public readonly record struct Month : IComparable<Month>, IComparable
{
    public static readonly Month January = new(1);
    public static readonly Month February = new(2);
    public static readonly Month March = new(3);
    public static readonly Month April = new(4);
    public static readonly Month May = new(5);
    public static readonly Month June = new(6);
    public static readonly Month July = new(7);
    public static readonly Month August = new(8);
    public static readonly Month September = new(9);
    public static readonly Month October = new(10);
    public static readonly Month November = new(11);
    public static readonly Month December = new(12);

    public int Value { get; }

    public Month(int value) =>
        this.Value = value >= 1 && value <= 12
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "The month value must be between 1 and 12");

     public override string ToString() =>
         this.Value.ToString("D2");

    public int CompareTo(Month other) =>
        this.Value.CompareTo(other.Value);

    readonly int IComparable.CompareTo(object? obj) =>
        obj != null ? this.CompareTo((Month)obj) : 1;
}

public static class MonthExtensions
{
    extension(int month)
    {
        public Month ToMonth() =>
            new(month);
    }
}
