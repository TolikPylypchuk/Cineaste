namespace Cineaste.Shared.Mapping;

public static class MappingExtensions
{
    public static ListItemModel ToListItemModel(this MovieModel movie) =>
        new(
            movie.Id,
            ListItemType.Movie,
            true,
            movie.DisplayNumber,
            movie.Titles.First().Name,
            movie.OriginalTitles.First().Name,
            movie.Year,
            movie.Year,
            movie.ListItemColor,
            movie.ParentFranchiseId is Guid franchiseId && movie.SequenceNumber is int num
                ? new(franchiseId, num)
                : null);

    public static ListItemModel ToListItemModel(this SeriesModel series) =>
        new(
            series.Id,
            ListItemType.Series,
            true,
            series.DisplayNumber,
            series.Titles.First().Name,
            series.OriginalTitles.First().Name,
            series.StartYear,
            series.EndYear,
            series.ListItemColor,
            series.ParentFranchiseId is Guid franchiseId && series.SequenceNumber is int num
                ? new(franchiseId, num)
                : null);

    public static ListItemModel ToListItemModel(this FranchiseModel franchise) =>
        new(
            franchise.Id,
            ListItemType.Franchise,
            franchise.ShowTitles,
            franchise.DisplayNumber,
            franchise.Titles.FirstOrDefault()?.Name ?? String.Empty,
            franchise.OriginalTitles.FirstOrDefault()?.Name ?? String.Empty,
            franchise.Items.Select(item => item.StartYear).Min(),
            franchise.Items.Select(item => item.EndYear).Max(),
            franchise.ListItemColor,
            franchise.ParentFranchiseId is Guid franchiseId && franchise.SequenceNumber is int num
                ? new(franchiseId, num)
                : null);
}
