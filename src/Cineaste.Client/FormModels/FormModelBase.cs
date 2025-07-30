using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using PropertyChanged;

namespace Cineaste.Client.FormModels;

public abstract partial class FormModelBase<TRequest, TModel> : INotifyPropertyChanged
    where TRequest : IValidatable<TRequest>
    where TModel : IIdentifyableModel
{
    private bool isInitialized = false;
    private TRequest request = default!;
    private TRequest? requestToCompare;

    [DoNotNotify]
    [DisallowNull]
    public TRequest? Request
    {
        get => this.ValidationErrors.Count == 0 ? this.request : default;
        private set => this.request = value;
    }

    [DoNotNotify]
    public TModel? BackingModel { get; private set; }

    [DoNotNotify]
    public IReadOnlySet<string> ValidationErrors { get; private set; } = ImmutableHashSet<string>.Empty;

    public bool HasChanges =>
        this.IsNew || !Equals(this.request, this.requestToCompare);

    public bool IsNew =>
        this.BackingModel is null || this.BackingModel is { Id: var id } && id == Guid.Empty;

    public void CopyFrom(TModel? model)
    {
        this.isInitialized = false;
        this.BackingModel = model;
        this.CopyFromModel();

        this.isInitialized = true;
        this.UpdateRequest();

        this.requestToCompare = this.request;
    }

    public virtual void UpdateRequest()
    {
        if (!this.isInitialized)
        {
            return;
        }

        this.request = this.CreateRequest();
        var validationResult = TRequest.Validator.Validate(request);

        this.ValidationErrors = validationResult.Errors.Select(error => error.ErrorCode).ToImmutableHashSet();
    }

    public abstract TRequest CreateRequest();

    protected abstract void CopyFromModel();

    protected void FinishInitialization()
    {
        this.isInitialized = true;
        this.PropertyChanged += (sender, e) => this.UpdateRequest();
        this.UpdateRequest();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}
