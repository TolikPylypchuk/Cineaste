using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Media.Imaging;

using Cineaste.Core;

using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste.Views
{
    public static class Extensions
    {
        public static IDisposable BindDefaultValidation<TView, TViewModel, T>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TViewModel, T?>> prop,
            Expression<Func<TView, string?>> errorText)
            where TView : class, IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            var subscriptions = new CompositeDisposable();

            view.BindValidation(viewModel, prop, errorText)
                .DisposeWith(subscriptions);
            
            view.WhenAnyValue(prop.PrependProperty<TViewModel, TView, T>(nameof(IViewFor.ViewModel)))
                .Take(1)
                .Select(_ => String.Empty)
                .BindTo(view, errorText)
                .DisposeWith(subscriptions);

            return subscriptions;
        }

        public static IDisposable BindDefaultValidation<TView, TViewModel>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TView, string?>> errorText)
            where TView : class, IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            var subscriptions = new CompositeDisposable();

            view.BindValidation(viewModel, errorText)
                .DisposeWith(subscriptions);

            var param = Expression.Parameter(typeof(TView));
            var vm = Expression.MakeMemberAccess(param, typeof(TView).GetProperty(nameof(IViewFor.ViewModel))!);

            view.WhenAnyValue(Expression.Lambda<Func<TView, TViewModel>>(vm, param)!)
                .Take(1)
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

        private static Expression<Func<TNewParam, T?>> PrependProperty<TParam, TNewParam, T>(
            this Expression<Func<TParam, T?>> originalExpression,
            string prop)
        {
            var param = Expression.Parameter(typeof(TNewParam));

            var newProperty = Expression.MakeMemberAccess(param, typeof(TNewParam).GetProperty(prop)!);

            var body = originalExpression.Body
                .GetExpressionChain()
                .OfType<MemberExpression>()
                .Reverse()
                .Aggregate(newProperty, (parent, expr) => Expression.MakeMemberAccess(parent, expr.Member));

            return Expression.Lambda<Func<TNewParam, T?>>(body, param);
        }
    }
}
