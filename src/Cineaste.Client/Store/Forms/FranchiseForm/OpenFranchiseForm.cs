using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Store.Forms.FranchiseForm;

public static class OpenFranchiseFormReducers
{
    [ReducerMethod]
    public static FranchiseFormState ReduceStartAddingFranchiseAction(
        FranchiseFormState _,
        StartAddingFranchiseAction action) =>
        new()
        {
            Fetch = ApiCall.Success(),
            InitialItemId = action.Item.Id,
            Model = new(
                Guid.Empty,
                [action.Title],
                [action.OriginalTitle],
                [action.Item],
                action.Kind,
                action.KindSource,
                ShowTitles: true,
                IsLooselyConnected: false,
                ContinueNumbering: false,
                ListItemColor: String.Empty,
                ListSequenceNumber: 0,
                FranchiseItem: null)
        };

    [ReducerMethod]
    public static FranchiseFormState ReduceSelectItemAction(FranchiseFormState state, SelectItemAction action) =>
        action.Item.Type == ListItemType.Franchise
            ? new() { Fetch = ApiCall.InProgress() }
            : state;

    [ReducerMethod(typeof(GoToListItemAction))]
    public static FranchiseFormState ReduceGoToListItemAction(FranchiseFormState _) =>
        new() { Fetch = ApiCall.Success() };

    [ReducerMethod(typeof(GoToFranchiseComponentAction))]
    public static FranchiseFormState ReduceGoToFranchiseComponentAction(FranchiseFormState _) =>
        new() { Fetch = ApiCall.Success() };
}

public sealed class OpenFranchiseFormEffects
{
    [EffectMethod]
    public Task HandleSelectItem(SelectItemAction action, IDispatcher dispatcher)
    {
        if (action.Item.Type == ListItemType.Franchise)
        {
            dispatcher.Dispatch(new FetchFranchiseAction(action.Item.Id));
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGoToListItemResult(GoToListItemResultAction action, IDispatcher dispatcher)
    {
        action.Handle(
            onSuccess: item =>
            {
                if (item.Type == ListItemType.Franchise)
                {
                    dispatcher.Dispatch(new FetchFranchiseAction(item.Id));
                }
            },
            onFailure: _ => { });

        return Task.CompletedTask;
    }
}
