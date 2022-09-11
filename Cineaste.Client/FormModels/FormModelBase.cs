namespace Cineaste.Client.FormModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using PropertyChanged;

public abstract partial class FormModelBase<TRequest, TModel> : INotifyPropertyChanged
    where TRequest : IValidatable<TRequest>
{
    private bool isInitialized = false;

    [DoNotNotify]
    public TRequest? Request { get; private set; }

    [DoNotNotify]
    public TModel? BackingModel { get; private set; }

    [DoNotNotify]
    public IReadOnlySet<string> ValidationErrors { get; private set; } = ImmutableHashSet<string>.Empty;

    public void CopyFrom(TModel? model)
    {
        this.BackingModel = model;
        this.CopyFromModel();
        this.UpdateRequest();
    }

    public virtual void UpdateRequest()
    {
        if (!this.isInitialized)
        {
            return;
        }

        var request = this.CreateRequest();
        var validationResult = TRequest.Validator.Validate(request);

        this.ValidationErrors = validationResult.Errors.Select(error => error.ErrorCode).ToImmutableHashSet();
        this.Request = this.ValidationErrors.Count == 0 ? request : default;
    }

    public abstract TRequest CreateRequest();

    protected abstract void CopyFromModel();

    protected void FinishInitialization()
    {
        this.isInitialized = true;
        this.PropertyChanged += (sender, e) => this.UpdateRequest();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}
