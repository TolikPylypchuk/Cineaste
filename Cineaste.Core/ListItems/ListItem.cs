using System;

using Cineaste.Core.Data.Models;
using Cineaste.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.ListItems
{
    public enum HighlightMode { None, Partial, Full }

    public abstract class ListItem : ReactiveObject, IEquatable<ListItem>
    {
        private protected ListItem(
            string id,
            FranchiseEntry? entry,
            string title,
            string originalTitle,
            string year,
            int startYearToCompare,
            int endYearToCompare,
            string color)
        {
            this.Id = id;
            this.DisplayNumber = entry.GetNumberToDisplay();
            this.Title = title;
            this.OriginalTitle = originalTitle;
            this.Year = year;
            this.StartYearToCompare = startYearToCompare;
            this.EndYearToCompare = endYearToCompare;
            this.Color = color;
        }

        public string Id { get; }

        public abstract FranchiseEntry? Entry { get; }

        [Reactive]
        public string DisplayNumber { get; set; }

        [Reactive]
        public string Title { get; set; }

        [Reactive]
        public string OriginalTitle { get; set; }

        [Reactive]
        public string Year { get; set; }

        [Reactive]
        public int StartYearToCompare { get; set; }

        [Reactive]
        public int EndYearToCompare { get; set; }

        [Reactive]
        public string Color { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }

        [Reactive]
        public HighlightMode HighlightMode { get; set; }

        public override bool Equals(object? obj) =>
            obj is ListItem item && this.Equals(item);

        public bool Equals(ListItem? other) =>
            !(other is null) && (ReferenceEquals(this, other) || this.Id == other.Id);

        public override int GetHashCode() =>
            this.Id.GetHashCode();

        public override string ToString() =>
            $"{this.Title} ({this.Id})";

        public static bool operator ==(ListItem? left, ListItem? right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(ListItem? left, ListItem? right) =>
            !(left == right);
    }
}
