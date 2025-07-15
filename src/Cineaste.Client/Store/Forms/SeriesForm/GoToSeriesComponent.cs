namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record GoToNextSeriesComponentAction(SeriesFormModel FormModel);

public sealed record GoToPreviousSeriesComponentAction(SeriesFormModel FormModel);

public static class GoToSeriesComponentReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceGoToNextSeriesComponentAction(
        SeriesFormState state,
        GoToNextSeriesComponentAction action) =>
        state.SelectedSeriesComponent is null
            ? state
            : state with
            {
                SelectedSeriesComponent = action.FormModel.Components
                    .FirstOrDefault(c => c.SequenceNumber == state.SelectedSeriesComponent.SequenceNumber + 1)
            };

    [ReducerMethod]
    public static SeriesFormState ReduceGoToPreviousSeriesComponentAction(
        SeriesFormState state,
        GoToPreviousSeriesComponentAction action) =>
        state.SelectedSeriesComponent is null
            ? state
            : state with
            {
                SelectedSeriesComponent = action.FormModel.Components
                    .FirstOrDefault(c => c.SequenceNumber == state.SelectedSeriesComponent.SequenceNumber - 1)
            };
}
