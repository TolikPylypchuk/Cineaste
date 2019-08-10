using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using HandyControl.Data;

using MovieList.Data.Models;
using MovieList.Properties;

namespace MovieList.ViewModels.FormItems
{
    public class TitleFormItem : FormItemBase
    {
        private string name;
        private int priority;
        private bool isOriginal;

        public TitleFormItem(Title title)
        {
            this.Title = title;
            this.CopyTitleProperties();
            this.IsInitialized = true;
        }

        public Title Title { get; }

        [Required(
            ErrorMessageResourceName = nameof(Messages.NameRequired),
            ErrorMessageResourceType = typeof(Messages))]
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

        public bool IsOriginal
        {
            get => this.isOriginal;
            set
            {
                this.isOriginal = value;
                this.OnPropertyChanged();
            }
        }

        public Func<string, OperationResult<bool>> VerifyName
            => this.Verify(nameof(this.Name));

        protected override IEnumerable<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)> Values
            => new List<(Func<object?> CurrentValueProvider, Func<object?> OriginalValueProvider)>
            {
                (() => this.Name, () => this.Title.Name),
                (() => this.Priority, () => this.Title.Priority),
                (() => this.IsOriginal, () => this.Title.IsOriginal)
            };

        public override void WriteChanges()
        {
            this.Title.Name = this.Name;
            this.Title.Priority = this.Priority;
            this.Title.IsOriginal = this.IsOriginal;
            this.AreChangesPresent = false;
        }

        public override void RevertChanges()
        {
            this.CopyTitleProperties();
            this.AreChangesPresent = false;
        }

        private void CopyTitleProperties()
        {
            this.Name = this.Title.Name;
            this.Priority = this.Title.Priority;
            this.IsOriginal = this.Title.IsOriginal;
        }
    }
}
