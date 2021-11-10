namespace Cineaste.Core.ViewModels.Forms.Preferences;

public class TagItemViewModel : ReactiveObject
{
    public TagItemViewModel(Tag tag, bool canSelect)
    {
        this.Tag = tag;
        this.CanSelect = canSelect;

        this.Name = this.Tag.Name;
        this.Category = this.Tag.Category;
        this.Description = this.Tag.Description;
        this.Color = this.Tag.Color;

        this.Select = ReactiveCommand.Create(() => { }, Observable.Return(canSelect));
        this.Delete = ReactiveCommand.Create(() => { });
    }

    public TagItemViewModel(TagModel tagModel, bool canSelect)
        : this(tagModel.Tag, canSelect)
    {
        this.TagModel = tagModel;

        this.WhenAnyValue(vm => vm.TagModel!.Name).BindTo(this, vm => vm.Name);
        this.WhenAnyValue(vm => vm.TagModel!.Description).BindTo(this, vm => vm.Description);
        this.WhenAnyValue(vm => vm.TagModel!.Category).BindTo(this, vm => vm.Category);
        this.WhenAnyValue(vm => vm.TagModel!.Color).BindTo(this, vm => vm.Color);
    }

    public Tag Tag { get; }
    public TagModel? TagModel { get; }

    [Reactive]
    public string Name { get; private set; }

    [Reactive]
    public string Description { get; private set; }

    [Reactive]
    public string Category { get; private set; }

    [Reactive]
    public string Color { get; private set; }

    public bool CanSelect { get; }

    public ReactiveCommand<Unit, Unit> Select { get; }
    public ReactiveCommand<Unit, Unit> Delete { get; }
}
