namespace Cineaste.Client.FormModels;

public abstract class IdentifyableFormModelBase<TRequest, TModel> : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>
    where TModel : IIdentifyableModel
{
    public override sealed bool HasChanges =>
        this.IsNew || base.HasChanges;

    public bool IsNew =>
        this.BackingModel is null || this.BackingModel is { Id: var id } && id == Guid.Empty;
}
