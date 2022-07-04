namespace Cineaste.Client.Validation;

public class RunValidationEventArgs : EventArgs
{
    public bool IsSuccessful { get; private set; } = true;

    public void ValidationFailed() =>
        this.IsSuccessful = false;
}
