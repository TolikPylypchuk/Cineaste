using System;
using System.Linq.Expressions;

using Cineaste.Core.Validation;
using Cineaste.Core.ViewModels.Forms.Base;

using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Cineaste.Core
{
    public static class ValidationExtensions
    {
        public static bool IsUrl(this string? str) =>
            String.IsNullOrEmpty(str) ||
                str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase);

        public static ValidationHelper ValidationRule<TViewModel>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, int>> viewModelProperty,
            int minValue,
            int maxValue,
            string? propertyName = null)
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            propertyName ??= viewModelProperty.GetMemberName();
            return viewModel.ValidationRule(
                viewModelProperty,
                value => value >= minValue && value <= maxValue,
                value => $"{propertyName}Invalid");
        }

        public static ValidationHelper ValidationRuleForColor<TForm>(
            this TForm form, Expression<Func<TForm, string?>> vmProperty)
            where TForm : ReactiveValidationObject, IReactiveForm =>
            form.ValidationRule(vmProperty, HexColorValidator.IsArgbString, "HexColorInvalid");
    }
}
