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
            this.SetProperties(entry, title, originalTitle, year, color);
        }

        public string Id { get; }

        [Reactive]
        public string DisplayNumber { get; set; } = null!;

        [Reactive]
        public string Title { get; set; } = null!;

        [Reactive]
        public string OriginalTitle { get; set; } = null!;

        [Reactive]
        public string Year { get; set; } = null!;

        [Reactive]
        public string Color { get; set; } = null!;

        public abstract void Refresh();

        public override string ToString()
            => $"{this.Title} ({this.Id})";

        protected void SetProperties(
            MovieSeriesEntry? entry,
            string title,
            string originalTitle,
            string year,
            string color)
        {
            this.DisplayNumber = entry.GetDisplayNumber();
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.Color = color;
        }
    }
}
