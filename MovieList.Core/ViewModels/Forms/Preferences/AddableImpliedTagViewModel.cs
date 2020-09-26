using MovieList.Core.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public sealed class AddableImpliedTagViewModel : ReactiveObject
    {
        public AddableImpliedTagViewModel(TagModel tagModel)
        {
            this.TagModel = tagModel;
            this.Name = tagModel.Name;
            this.Category = tagModel.Category;
        }

        public TagModel TagModel { get; }

        public string Name { get; }
        public string Category { get; }
    }
}
