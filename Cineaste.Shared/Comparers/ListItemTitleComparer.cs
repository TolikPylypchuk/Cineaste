namespace Cineaste.Shared.Comparers;

using System.Globalization;

using Nito.Comparers;

public sealed class ListItemTitleComparer : NullableComparerBase<ListItemModel>
{
    private readonly IComparer<ListItemModel> titleComparer;
    private readonly Func<Guid, ListItemModel> getItem;
    private readonly Func<ListItemModel, string> getTitle;

    public ListItemTitleComparer(
        CultureInfo culture,
        IComparer<ListItemModel> otherComparer,
        Func<Guid, ListItemModel> getItem,
        Func<ListItemModel, string> getTitle,
        NullComparison nullComparison = NullComparison.NullsFirst)
        : base(nullComparison)
    {
        this.getItem = getItem;
        this.getTitle = getTitle;

        this.titleComparer = ComparerBuilder.For<ListItemModel>()
            .OrderBy(this.getTitle, new TitleComparer(culture))
            .ThenBy(otherComparer);
    }

    protected override bool EqualsSafe(ListItemModel x, ListItemModel y) =>
        x == y;

    protected override int GetHashCodeSafe(ListItemModel item) =>
        item.GetHashCode();

    protected override int CompareSafe(ListItemModel x, ListItemModel y) =>
        (x.Type, y.Type) switch
        {
            (ListItemType.Movie, ListItemType.Movie) => this.CompareConsideringFranchiseItems(x, y),
            (ListItemType.Movie, ListItemType.Series) => this.CompareConsideringFranchiseItems(x, y),
            (ListItemType.Movie, ListItemType.Franchise) => this.CompareWithFranchise(y, x) * -1,

            (ListItemType.Series, ListItemType.Movie) => this.CompareConsideringFranchiseItems(x, y),
            (ListItemType.Series, ListItemType.Series) => this.CompareConsideringFranchiseItems(x, y),
            (ListItemType.Series, ListItemType.Franchise) => this.CompareWithFranchise(y, x) * -1,

            (ListItemType.Franchise, ListItemType.Movie) => this.CompareWithFranchise(x, y),
            (ListItemType.Franchise, ListItemType.Series) => this.CompareWithFranchise(x, y),
            (ListItemType.Franchise, ListItemType.Franchise) => this.CompareFranchises(x, y),

            _ => throw new NotSupportedException(
                $"Types of list items to compare are not supported: {x.Type}, {y.Type}")
        };

    private int CompareConsideringFranchiseItems(ListItemModel left, ListItemModel right) =>
        left.Id == right.Id && left.Type == right.Type
            ? 0
            : (left.FranchiseItem, right.FranchiseItem) switch
            {
                (null, null) => this.CompareTitle(left, right),
                (_, null) => this.CompareWithFranchise(this.GetRootFranchise(left), right),
                (null, _) => this.CompareWithFranchise(this.GetRootFranchise(right), left) * -1,
                (var item1, var item2) => item1.FranchiseId == item2.FranchiseId
                    ? item1.SequenceNumber.CompareTo(item2.SequenceNumber)
                    : this.CompareFranchiseItems(item1, item2)
            };

    private int CompareFranchises(ListItemModel left, ListItemModel right)
    {
        if (left.Id == right.Id)
        {
            return 0;
        } else if (this.IsDescendant(left, right))
        {
            return 1;
        } else if (this.IsDescendant(right, left))
        {
            return -1;
        } else if (this.GetRootFranchise(left).Id == this.GetRootFranchise(left).Id)
        {
            var (ancestor1, ancestor2) = this.GetDistinctAncestors(left, right);
            return ancestor1 is { FranchiseItem.SequenceNumber: int num1 } &&
                ancestor2 is { FranchiseItem.SequenceNumber: int num2 }
                ? num1.CompareTo(num2)
                : 0;
        } else if (left.FranchiseItem is null && right.FranchiseItem is null)
        {
            return this.CompareTitle(left, right);
        } else
        {
            return this.CompareTitle(this.GetRootFranchise(left), this.GetRootFranchise(right));
        }
    }

    private int CompareWithFranchise(ListItemModel franchise, ListItemModel model)
    {
        if (model.FranchiseItem == null)
        {
            return this.CompareTitle(this.GetRootFranchise(franchise), model);
        }

        if (franchise.Id == model.FranchiseItem.FranchiseId)
        {
            return -1;
        }

        var modelFranchise = this.getItem(model.FranchiseItem.FranchiseId);

        if (this.IsStrictDescendant(franchise, modelFranchise))
        {
            return this.FindSequenceNumberToCompare(descendant: franchise, ancestor: modelFranchise)
                .CompareTo(model.FranchiseItem.SequenceNumber);
        }

        return this.CompareFranchises(franchise, modelFranchise);
    }

    private int CompareFranchiseItems(ListFranchiseItemModel left, ListFranchiseItemModel right)
    {
        var leftFranchise = this.getItem(left.FranchiseId);
        var rightFranchise = this.getItem(right.FranchiseId);

        if (this.IsStrictDescendant(leftFranchise, rightFranchise))
        {
            return this.FindSequenceNumberToCompare(descendant: leftFranchise, ancestor: rightFranchise)
                .CompareTo(right.SequenceNumber);
        } else if (this.IsStrictDescendant(rightFranchise, leftFranchise))
        {
            return left.SequenceNumber.CompareTo(
                this.FindSequenceNumberToCompare(descendant: rightFranchise, ancestor: leftFranchise));
        }

        return this.CompareFranchises(leftFranchise, rightFranchise);
    }

    private int CompareTitle(ListItemModel left, ListItemModel right) =>
        this.titleComparer.Compare(left, right);

    private int FindSequenceNumberToCompare(ListItemModel descendant, ListItemModel ancestor) =>
        this.GetAllAncestors(descendant)
            .First(a => a.FranchiseItem is not null && a.FranchiseItem.FranchiseId == ancestor.Id)
            .FranchiseItem!
            .SequenceNumber;

    private ListItemModel GetRootFranchise(ListItemModel model) =>
        model.FranchiseItem is { FranchiseId: var id } ? this.GetRootFranchise(this.getItem(id)) : model;

    private bool IsDescendant(ListItemModel model, ListItemModel ancestor) =>
        model == ancestor ||
            model.FranchiseItem is { FranchiseId: var id } && this.IsDescendant(this.getItem(id), ancestor);

    private bool IsStrictDescendant(ListItemModel model, ListItemModel ancestor) =>
        model != ancestor && this.IsDescendant(model, ancestor);

    private IEnumerable<ListItemModel> GetAllAncestors(ListItemModel model)
    {
        if (model == null)
        {
            yield break;
        }

        if (model.FranchiseItem is { FranchiseId: var id })
        {
            foreach (var ancestor in this.GetAllAncestors(this.getItem(id)))
            {
                yield return ancestor;
            }
        }

        yield return model;
    }

    private (ListItemModel, ListItemModel) GetDistinctAncestors(ListItemModel left, ListItemModel right) =>
        this.GetAllAncestors(left)
            .Zip(this.GetAllAncestors(right), (a, b) => (Franchise1: a, Franchise2: b))
            .First(ancestors => ancestors.Franchise1.Id != ancestors.Franchise2.Id);
}
