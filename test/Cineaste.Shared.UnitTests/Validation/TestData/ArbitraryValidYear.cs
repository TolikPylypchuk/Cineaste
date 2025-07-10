namespace Cineaste.Shared.Validation.TestData;

public class ArbitraryValidYear : Arbitrary<int>
{
    public override Gen<int> Generator =>
        Gen.Choose(MinYear + 1, 20_000);
}
