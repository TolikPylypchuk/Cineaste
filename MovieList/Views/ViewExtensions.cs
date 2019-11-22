using System;
using System.Windows;

using MovieList.Validation;

using ReactiveUI.Validation.Helpers;

namespace MovieList.Views
{
    public static class ViewExtensions
    {
        public static IDisposable ValidateWith(this FrameworkElement element, ValidationHelper rule)
            => ValidationSubscriber.Create(element, rule);
    }
}
