namespace Cineaste.Client.Store.Forms;

public sealed record StartAddingMovieAction(Guid? ParentFranchiseId = null);

public sealed record StartAddingSeriesAction(Guid? ParentFranchiseId = null);

public sealed record StartAddingFranchiseAction(Guid? ParentFranchiseId = null);

public sealed record StartAddingParentFranchiseAction(
    TitleModel Title,
    TitleModel OriginalTitle,
    FranchiseItemModel Item,
    ListKindModel Kind,
    FranchiseKindSource KindSource);
