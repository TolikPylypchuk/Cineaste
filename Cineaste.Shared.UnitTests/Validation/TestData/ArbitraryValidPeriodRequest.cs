namespace Cineaste.Shared.Validation.TestData;

using Cineaste.Shared.Models.Series;

using FsCheck;
using FsCheck.Fluent;

public class ArbitraryValidPeriodRequest : Arbitrary<PeriodRequest>
{
    public override Gen<PeriodRequest> Generator =>
        from month1 in Gen.Choose(1, 12)
        from month2 in Gen.Choose(1, 12)
        from year1 in Gen.Choose(2000, 2002)
        from year2 in Gen.Choose(2000, 2002)
        let startMonth = Math.Min(month1, month2)
        let endMonth = Math.Max(month1, month2)
        let startYear = Math.Min(year1, year2)
        let endYear = Math.Max(year1, year2)
        select new PeriodRequest(null, startMonth, startYear, endMonth, endYear, 5, false, null);
}
