namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record GoToSeriesAction;

public static class GoToSeriesReducers
{
    [ReducerMethod(typeof(GoToSeriesAction))]
    public static SeriesFormState ReduceGoToSeriesAction(SeriesFormState state) =>
        state with { SelectedSeriesComponent = null };
}
