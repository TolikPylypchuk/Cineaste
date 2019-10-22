using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

using ReactiveUI;

using Expression = System.Linq.Expressions.Expression;

namespace MovieList.Infrastructure
{
    public class CustomPropertyResolver : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(type))
            {
                return 0;
            }

            var field = type.GetTypeInfo()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(f => f.Name == propertyName);

            return field != null ? 2 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(
            object sender,
            Expression expression,
            string propertyName,
            bool beforeChanged = false,
            bool suppressWarnings = false)
        {
            var foo = (FrameworkElement)sender;
            return Observable.Return(
                    new ObservedChange<object, object>(sender, expression), new DispatcherScheduler(foo.Dispatcher))
                .Concat(Observable.Never<IObservedChange<object, object>>());
        }
    }
}
