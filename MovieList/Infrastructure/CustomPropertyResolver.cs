using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

using ReactiveUI;

namespace MovieList.Infrastructure
{
    public class CustomPropertyResolver : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false) => 2;

        public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
            object sender,
            Expression expression,
            string propertyName,
            bool beforeChanged = false,
            bool suppressWarnings = false)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            return Observable.Return(
                new ObservedChange<object, object?>(sender, expression, default), RxApp.MainThreadScheduler)
                .Concat(Observable.Never<IObservedChange<object, object?>>());
        }
    }
}
