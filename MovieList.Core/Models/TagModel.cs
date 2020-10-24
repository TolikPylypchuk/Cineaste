using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.Models
{
    public sealed class TagModel : ReactiveObject
    {
        public TagModel(Tag tag)
        {
            this.Tag = tag;
            this.Name = tag.Name;
            this.Description = tag.Description;
            this.Category = tag.Category;
            this.Color = tag.Color;
            this.IsApplicableToMovies = tag.IsApplicableToMovies;
            this.IsApplicableToSeries = tag.IsApplicableToSeries;
        }

        public Tag Tag { get; }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public string Description { get; set; }

        [Reactive]
        public string Category { get; set; }

        [Reactive]
        public string Color { get; set; }

        [Reactive]
        public bool IsApplicableToMovies { get; set; }

        [Reactive]
        public bool IsApplicableToSeries { get; set; }

        public ObservableCollection<TagModel> ImpliedTags { get; } = new();

        public IEnumerable<TagModel> GetImpliedTagsClosure()
            => GetClosure(new[] { this });

        public override string ToString()
            => $"Tag model: {this.Name} ({this.Category})";

        private static IEnumerable<TagModel> GetClosure(IEnumerable<TagModel> tags)
            => tags.Union(tags.SelectMany(tag => GetClosure(tag.ImpliedTags)));
    }
}
