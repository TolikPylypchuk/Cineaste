using Cineaste.Shared.Models.Series;

namespace Cineaste.Shared.Validation.TestData;

public class ArbitraryValidSeasonPartRequest : Arbitrary<SeasonPartRequest>
{
    public override Gen<SeasonPartRequest> Generator =>
        from month1 in Gen.Choose(1, 12)
        from month2 in Gen.Choose(1, 12)
        from year1 in Gen.Choose(2000, 2002)
        from year2 in Gen.Choose(2000, 2002)
        let startMonth = Math.Min(month1, month2)
        let endMonth = Math.Max(month1, month2)
        let startYear = Math.Min(year1, year2)
        let endYear = Math.Max(year1, year2)
        select new SeasonPartRequest(null, new(startMonth, startYear, endMonth, endYear, 5, false), null);
}
