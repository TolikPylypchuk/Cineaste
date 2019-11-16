using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ListItems
{
    public abstract class ListItem : ReactiveObject
    {
        protected ListItem(
            string id,
            MovieSeriesEntry? entry,
            string title,
            string originalTitle,
            string year,
            string color)
        {
            this.Id = id;
            this.DisplayNumber = entry.GetDisplayNumber();
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.Color = color;
        }

        public string Id { get; }

        [Reactive]
        public string DisplayNumber { get; set; }

        [Reactive]
        public string Title { get; set; }

        [Reactive]
        public string OriginalTitle { get; set; }

        [Reactive]
        public string Year { get; set; }

        [Reactive]
        public string Color { get; set; }

        public override string ToString()
            => $"{this.Title} ({this.Id})";
    }
}
