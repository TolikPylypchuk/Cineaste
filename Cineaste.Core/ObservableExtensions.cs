using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Cineaste.Core
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> Discard<T>(this IObservable<T> obserevable) =>
            obserevable.Select(_ => Unit.Default);

        public static IObservable<T> WhereValueNotNull<T>(this IObservable<T?> observable)
            where T : struct =>
            observable.Where(value => value.HasValue).Select(value => value!.Value);

        public static IObservable<bool> AllTrue(this IObservable<IEnumerable<bool>> observable) =>
            observable.Select(values => values.All(value => value));

        public static IObservable<bool> AnyTrue(this IObservable<IEnumerable<bool>> observable) =>
            observable.Select(values => values.Any(value => value));

        public static IObservable<TResult?> SelectNotNull<T, TResult>(
            this IObservable<T?> observable,
            Func<T, TResult> projection)
            where T : class
            where TResult : class =>
            observable.Select(x => x is null ? null : projection(x));

        public static IObservable<bool> Invert(this IObservable<bool> observable) =>
            observable.Select(value => !value);

        public static IObservable<T> Eager<T>(this IObservable<T> observable)
        {
            var connectableObservable = observable.Publish();
            connectableObservable.Connect();
            return connectableObservable;
        }

        public static IObservable<T> Select<T>(this IObservable<Unit> observable, Func<T> projection) =>
            observable.Select(_ => projection());

        public static IObservable<T> SelectMany<T>(
            this IObservable<Unit> observable,
            Func<IObservable<T>> projection) =>
            observable.SelectMany(_ => projection());

        public static IObservable<Unit> Do(this IObservable<Unit> observable, Action action) =>
            observable.Do(_ => action());

        public static IObservable<Unit> DoAsync<TAny>(
            this IObservable<Unit> observable,
            Func<IObservable<TAny>> action) =>
            observable.SelectMany(_ => action().Select(_ => Unit.Default));

        public static IObservable<T> DoAsync<T, TAny>(
            this IObservable<T> observable,
            Func<T, IObservable<TAny>> action) =>
            observable.SelectMany(result => action(result).Select(_ => result));

        public static IObservable<bool> DoIfTrue(this IObservable<bool> observable, Action action) =>
            observable.Do(value =>
            {
                if (value)
                {
                    action();
                }
            });

        public static IObservable<bool> DoIfTrueAsync<TAny>(
            this IObservable<bool> observable,
            Func<IObservable<TAny>> action) =>
            observable.SelectMany(value => value
                ? action().Select(_ => value)
                : Observable.Return(value));

        public static IDisposable Subscribe(this IObservable<Unit> observable, Action observer) =>
            observable.Subscribe(_ => observer());

        public static IDisposable SubscribeAsync<T>(
            this IObservable<T> observable,
            Func<T, IObservable<Unit>> observer) =>
            observable.SelectMany(value => observer(value)).Subscribe();

        public static IDisposable SubscribeAsync(this IObservable<Unit> observable, Func<IObservable<Unit>> observer) =>
            observable.SelectMany(_ => observer()).Subscribe();
    }
}
