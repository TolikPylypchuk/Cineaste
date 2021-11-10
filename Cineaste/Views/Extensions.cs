namespace Cineaste.Views;

using System.Linq.Expressions;

using ReactiveUI.Validation.Abstractions;

using ComboBox = Avalonia.Controls.ComboBox;

public static class Extensions
{
    public static IDisposable BindStrictValidation<TView, TViewModel, T>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, T?>> prop,
        Expression<Func<TView, string?>> errorText,
        bool magicallyWorks = true)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IValidatableViewModel
    {
        var subscriptions = new CompositeDisposable();

        view.BindValidation(viewModel, prop, errorText)
            .DisposeWith(subscriptions);

        view.WhenAnyValue(errorText)
            .DistinctUntilChanged()
            .Where(error => !String.IsNullOrEmpty(error))
            .Take(magicallyWorks ? 1 : 4)
            .Select(_ => String.Empty)
            .BindTo(view, errorText)
            .DisposeWith(subscriptions);

        return subscriptions;
    }

    public static void SetEnumValues<TEnum>(this ComboBox comboBox, params TEnum[] values)
        where TEnum : struct, Enum
    {
        var converter = GetDefaultService<IEnumConverter<TEnum>>();

        comboBox.Items = (values.Length != 0 ? values : Enum.GetValues<TEnum>())
            .Select(converter.ToString)
            .ToList();
    }

    public static IBitmap? AsImage([AllowNull] this byte[] imageData) =>
        imageData == null || imageData.Length == 0 ? null : new Bitmap(new MemoryStream(imageData));
}
