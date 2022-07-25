namespace Cineaste.Client.Store.Forms;

public sealed class ApiCall
{
    public bool IsInProgress { get; }
    public bool IsFinished { get; }
    public ProblemDetails? Problem { get; }

    private ApiCall(bool isInProgress, bool isFinished, ProblemDetails? problem)
    {
        this.IsInProgress = isInProgress;
        this.IsFinished = isFinished;
        this.Problem = problem;
    }

    public static ApiCall NotStarted() =>
        new(false, false, null);

    public static ApiCall InProgress() =>
        new(true, false, null);

    public static ApiCall Success() =>
        new(false, true, null);

    public static ApiCall Failure(ProblemDetails problem) =>
        new(false, true, problem);
}
