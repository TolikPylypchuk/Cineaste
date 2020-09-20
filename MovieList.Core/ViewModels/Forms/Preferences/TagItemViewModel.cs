using System.Reactive;
using System.Reactive.Linq;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Forms.Preferences
{
    public class TagItemViewModel : ReactiveObject
    {
        public TagItemViewModel(Tag tag, bool canSelect, bool canDelete)
        {
            this.Form = new TagFormViewModel(tag);
            this.CanSelect = canSelect;
            this.CanDelete = canDelete;
            this.SetProperties();

            this.Select = ReactiveCommand.Create(() => { }, Observable.Return(canSelect));
            this.Refresh = ReactiveCommand.Create(this.SetProperties);
            this.Delete = ReactiveCommand.Create(() => { }, Observable.Return(canDelete));
        }

        public TagFormViewModel Form { get; }

        [Reactive]
        public string Name { get; set; } = null!;

        [Reactive]
        public string Category { get; set; } = null!;

        [Reactive]
        public string Description { get; set; } = null!;

        [Reactive]
        public string Color { get; set; } = null!;

        public bool CanSelect { get; }
        public bool CanDelete { get; }

        public ReactiveCommand<Unit, Unit> Select { get; }
        public ReactiveCommand<Unit, Unit> Refresh { get; }
        public ReactiveCommand<Unit, Unit> Delete { get; }

        private void SetProperties()
        {
            this.Name = this.Form.Name;
            this.Category = this.Form.Category;
            this.Description = this.Form.Description;
            this.Color = this.Form.Color;
        }
    }
}
