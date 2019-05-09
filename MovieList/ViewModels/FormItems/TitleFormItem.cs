using System;
using System.Collections.Generic;
using MovieList.Data.Models;

namespace MovieList.ViewModels.FormItems
{
    public class TitleFormItem : FormItemBase
    {
        private string name;
        private int priority;

        public TitleFormItem(Title title)
        {
            this.Title = title;

            this.name = title.Name;
            this.priority = title.Priority;
        }

        public Title Title { get; }

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public int Priority
        {
            get => this.priority;
            set
            {
                this.priority = value;
                this.OnPropertyChanged();
            }
        }

        protected override List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Name, () => this.Title.Name),
                (() => this.Priority, () => this.Title.Priority)
            };

        public override void WriteChanges()
        {
            this.Title.Name = this.Name;
            this.Title.Priority = this.Priority;
        }
    }
}
