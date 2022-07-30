namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record OpenSeriesComponentFormAction(SeriesComponentModel Component);

public static class OpenSeriesComponentFormReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceOpenSeriesComponentFormAction(
        SeriesFormState state,
        OpenSeriesComponentFormAction action) =>
        state with { SelectedSeriesComponent = action.Component };
}
