using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class AddableImpliedTagViewModel : ReactiveObject
    {
        public AddableImpliedTagViewModel(TagItemViewModel tag)
        {
            this.Tag = tag;
            this.Name = tag.Name;
            this.Category = tag.Category;
        }

        public TagItemViewModel Tag { get; }

        public string Name { get; }
        public string Category { get; }
    }
}
