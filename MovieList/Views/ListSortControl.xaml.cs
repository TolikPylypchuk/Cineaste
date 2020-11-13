using System.ComponentModel;
using System.Reactive.Disposables;

using MovieList.Core.ViewModels;
using MovieList.Data;

using ReactiveUI;

using static MovieList.Data.ListSortOrder;

namespace MovieList.Views
{
    public abstract class ListSortControlBase : ReactiveUserControl<ListSortViewModel> { }

    public partial class ListSortControl : ListSortControlBase
    {
        public ListSortControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.FirstOrderComboBox.AddEnumValues<ListSortOrder>();
                this.FirstDirectionComboBox.AddEnumValues<ListSortDirection>();
                this.SecondOrderComboBox.AddEnumValues(ByTitleSimple, ByOriginalTitleSimple, ByYear);
                this.SecondDirectionComboBox.AddEnumValues<ListSortDirection>();

                this.Bind(this.ViewModel!, vm => vm.FirstOrder, v => v.FirstOrderComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel!, vm => vm.FirstDirection, v => v.FirstDirectionComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel!, vm => vm.SecondOrder, v => v.SecondOrderComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(this.ViewModel!, vm => vm.SecondDirection, v => v.SecondDirectionComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Apply, v => v.ApplyButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
