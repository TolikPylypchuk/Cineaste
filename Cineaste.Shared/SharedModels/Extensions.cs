namespace Cineaste.Shared.SharedModels;

public static class Extensions
{
    public static SimpleKindModel ToSimpleModel(this ListKindModel model) =>
        new(model.Id, model.Name, model.Target);
}
