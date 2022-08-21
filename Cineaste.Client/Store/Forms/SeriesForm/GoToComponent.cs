namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record GoToNextComponentAction(SeriesFormModel FormModel);

public sealed record GoToPreviousComponentAction(SeriesFormModel FormModel);

public static class GoToComponentReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceGoToNextComponentAction(
        SeriesFormState state,
        GoToNextComponentAction action) =>
        state.SelectedSeriesComponent is null
            ? state
            : state with
            {
                SelectedSeriesComponent = action.FormModel.Components
                    .FirstOrDefault(c => c.SequenceNumber == state.SelectedSeriesComponent.SequenceNumber + 1)
            };

    [ReducerMethod]
    public static SeriesFormState ReduceGoToPreviousComponentAction(
        SeriesFormState state,
        GoToPreviousComponentAction action) =>
        state.SelectedSeriesComponent is null
            ? state
            : state with
            {
                SelectedSeriesComponent = action.FormModel.Components
                    .FirstOrDefault(c => c.SequenceNumber == state.SelectedSeriesComponent.SequenceNumber - 1)
            };
}
