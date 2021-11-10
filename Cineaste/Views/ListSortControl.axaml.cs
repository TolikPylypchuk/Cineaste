namespace Cineaste.Views;

public partial class ListSortControl : ReactiveUserControl<ListSortViewModel>
{
    public ListSortControl()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .BindTo(this, v => v.DataContext)
                .DisposeWith(disposables);

            this.FirstOrderComboBox.SetEnumValues<ListSortOrder>();
            this.FirstDirectionComboBox.SetEnumValues<ListSortDirection>();
            this.SecondOrderComboBox.SetEnumValues(ByTitleSimple, ByOriginalTitleSimple, ByYear);
            this.SecondDirectionComboBox.SetEnumValues<ListSortDirection>();

            this.Bind(this.ViewModel, vm => vm.FirstOrder, v => v.FirstOrderComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.FirstDirection, v => v.FirstDirectionComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.SecondOrder, v => v.SecondOrderComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.Bind(this.ViewModel, vm => vm.SecondDirection, v => v.SecondDirectionComboBox.SelectedItem)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Apply, v => v.ApplyButton)
                .DisposeWith(disposables);
        });
    }
}
