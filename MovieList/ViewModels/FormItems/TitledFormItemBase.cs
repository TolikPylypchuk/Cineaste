using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public abstract class TitledFormItemBase : FormItemBase
    {
        private ObservableCollection<TitleFormItem> titles;
        private ObservableCollection<TitleFormItem> originalTitles;

        public TitledFormItemBase()
        {
            this.AddTitle = new DelegateCommand(
                _ => this.OnAddTitle(), _ => this.CanAddTitle());

            this.AddOriginalTitle = new DelegateCommand(
                _ => this.OnAddOriginalTitle(), _ => this.CanAddOriginalTitle());

            this.RemoveTitle = new DelegateCommand(
                this.OnRemoveTitle, _ => this.CanRemoveTitle());

            this.RemoveOriginalTitle = new DelegateCommand(
                this.OnRemoveOriginalTitle, _ => this.CanRemoveOriginalTitle());
        }

        public ICommand AddTitle { get; }
        public ICommand RemoveTitle { get; }
        public ICommand AddOriginalTitle { get; }
        public ICommand RemoveOriginalTitle { get; }

        public ObservableCollection<TitleFormItem> Titles
        {
            get => this.titles;
            set
            {
                this.titles = value;
                this.titles.CollectionChanged += this.OnTitlesChanged;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<TitleFormItem> OriginalTitles
        {
            get => this.originalTitles;
            set
            {
                this.originalTitles = value;
                this.originalTitles.CollectionChanged += this.OnTitlesChanged;
                this.OnPropertyChanged();
            }
        }

        public void ClearEmptyTitles()
        {
            bool shouldContinue = true;

            while (this.Titles.Count != 1 && shouldContinue)
            {
                var title = this.Titles.FirstOrDefault(t => String.IsNullOrEmpty(t.Name));
                this.Titles.Remove(title);
                shouldContinue = title != null;
            }

            while (this.OriginalTitles.Count != 1 && shouldContinue)
            {
                var title = this.OriginalTitles.FirstOrDefault(t => String.IsNullOrEmpty(t.Name));
                this.OriginalTitles.Remove(title);
                shouldContinue = title != null;
            }
        }

        public void CopyTitles(IEnumerable<Title> titles)
        {
            this.Titles = new ObservableCollection<TitleFormItem>(
                from title in titles
                where !title.IsOriginal
                select new TitleFormItem(title));

            this.Titles.CollectionChanged += this.OnTitlesChanged;

            this.OriginalTitles = new ObservableCollection<TitleFormItem>(
                from title in titles
                where title.IsOriginal
                select new TitleFormItem(title));

            this.OriginalTitles.CollectionChanged += this.OnTitlesChanged;

            foreach (var title in this.Titles)
            {
                title.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.Titles));
            }

            foreach (var title in this.OriginalTitles)
            {
                title.PropertyChanged += (sender, e) => this.OnPropertyChanged(nameof(this.OriginalTitles));
            }
        }

        private void OnAddTitle()
        {
            this.Titles.Add(new TitleFormItem(
                new Title { IsOriginal = false, Priority = this.Titles.Count + 1 }));
            this.OnPropertyChanged(nameof(this.Titles));
        }

        private bool CanAddTitle()
            => this.Titles.Count < 10;

        private void OnAddOriginalTitle()
        {
            this.OriginalTitles.Add(new TitleFormItem(
                new Title { IsOriginal = true, Priority = this.OriginalTitles.Count + 1 }));
            this.OnPropertyChanged(nameof(this.OriginalTitles));
        }

        private bool CanAddOriginalTitle()
            => this.OriginalTitles.Count < 10;

        private void OnRemoveTitle(object obj)
        {
            if (obj is TitleFormItem title)
            {
                this.Titles.Remove(title);

                foreach (var t in this.Titles.Where(t => t.Priority > title.Priority))
                {
                    t.Priority--;
                }
            }
        }

        private bool CanRemoveTitle()
            => this.Titles.Count != 1;

        private void OnRemoveOriginalTitle(object obj)
        {
            if (obj is TitleFormItem title)
            {
                this.OriginalTitles.Remove(title);

                foreach (var t in this.OriginalTitles.Where(t => t.Priority > title.Priority))
                {
                    t.Priority--;
                }
            }
        }

        private bool CanRemoveOriginalTitle()
            => this.OriginalTitles.Count != 1;

        private void OnTitlesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var title in e.NewItems.OfType<TitleFormItem>())
                {
                    title.PropertyChanged += (s, e) => this.OnPropertyChanged(
                        sender == this.titles ? nameof(this.Titles) : nameof(this.OriginalTitles));
                }
            }

            this.OnPropertyChanged(sender == this.titles ? nameof(this.Titles) : nameof(this.OriginalTitles));
        }
    }
}
