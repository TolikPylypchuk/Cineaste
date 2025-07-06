namespace Cineaste.Shared.Validation.TestData;

using FsCheck;
using FsCheck.Fluent;

public class ArbitraryValidMonth : Arbitrary<byte>
{
    public override Gen<byte> Generator =>
        from month in Gen.Choose(1, 12)
        select (byte)month;
}
