using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Resources;

using DynamicData;
using DynamicData.Binding;

using MovieList.Core;
using MovieList.Core.ViewModels.Forms.Preferences;
using MovieList.Data.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public abstract class TagFormBase<TModel, TForm> : ReactiveForm<TModel, TForm>
        where TModel : class
        where TForm : TagFormBase<TModel, TForm>
    {
        private readonly ReadOnlyObservableCollection<TagItemViewModel> impliedTags;

        public TagFormBase(ResourceManager? resourceManager = null, IScheduler? scheduler = null)
            : base(resourceManager, scheduler)
        {
            this.Name = String.Empty;
            this.Description = String.Empty;
            this.Category = String.Empty;
            this.Color = String.Empty;

            this.ImpliedTagsSource.Connect()
                .Transform(this.CreateTagItemViewModel)
                .Sort(SortExpressionComparer<TagItemViewModel>
                    .Ascending(vm => vm.Category)
                    .ThenByAscending(vm => vm.Name))
                .Bind(out this.impliedTags)
                .DisposeMany()
                .Subscribe();
        }

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

        [Reactive]
        public bool IsApplicableToFranchises { get; set; }

        public ReadOnlyObservableCollection<TagItemViewModel> ImpliedTags
            => this.impliedTags;

        protected SourceList<Tag> ImpliedTagsSource { get; } = new();

        private TagItemViewModel CreateTagItemViewModel(Tag tag)
        {
            var viewModel = new TagItemViewModel(tag, canSelect: false);

            var subscriptions = new CompositeDisposable();

            viewModel.Delete
                .Subscribe(_ =>
                {
                    this.ImpliedTagsSource.Remove(tag);
                    subscriptions.Dispose();
                })
                .DisposeWith(subscriptions);

            return viewModel;
        }
    }
}
