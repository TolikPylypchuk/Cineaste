namespace Cineaste.Core.ViewModels.Forms.Preferences;

public sealed class AddableTagViewModel : ReactiveObject
{
    public AddableTagViewModel(Tag tag)
    {
        this.Tag = tag;
        this.Name = tag.Name;
        this.Category = tag.Category;
        this.Color = tag.Color;
    }

    public AddableTagViewModel(TagModel tagModel)
    {
        this.Tag = tagModel.Tag;
        this.TagModel = tagModel;
        this.Name = tagModel.Name;
        this.Category = tagModel.Category;
        this.Color = tagModel.Color;
    }


    public Tag Tag { get; }
    public TagModel? TagModel { get; }

    public string Name { get; }
    public string Category { get; }
    public string Color { get; }
}
