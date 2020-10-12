using System.Reactive.Disposables;

using MovieList.Core.ViewModels.Filters;

using ReactiveUI;

namespace MovieList.Views.Filters
{
    public abstract class TagsFilterInputControlBase : ReactiveUserControl<TagsFilterInputViewModel> { }

    public partial class TagsFilterInputControl : TagsFilterInputControlBase
    {
        public TagsFilterInputControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
