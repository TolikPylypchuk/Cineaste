namespace Cineaste.Client.Validation;

public class ExecuteValidationEventArgs : EventArgs
{
    public bool IsSuccessful { get; private set; } = true;

    public void ValidationFailed() =>
        this.IsSuccessful = false;
}
