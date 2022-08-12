namespace Cineaste.Client.FormModels;

public abstract class FormModel<TModel>
{
    public TModel? BackingModel { get; private set; }

    public void CopyFrom(TModel? model)
    {
        this.BackingModel = model;
        this.CopyFromModel();
    }

    protected abstract void CopyFromModel();
}
