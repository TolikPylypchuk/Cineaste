using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MovieList
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> Discard<T>(this IObservable<T> obserevable)
            => obserevable.Select(_ => Unit.Default);

        public static IObservable<T> WhereNotNull<T>(this IObservable<T?> observable)
            where T : class
            => observable.Where(x => x != null).Select(x => x!);

        public static IDisposable Subscribe(this IObservable<Unit> observable, Action observer)
            => observable.Subscribe(_ => observer());

        public static IDisposable SubscribeAsync<T>(this IObservable<T> observable, Func<T, Task> observer)
            => observable.SelectMany(async x =>
            {
                await observer(x);
                return Unit.Default;
            }).Subscribe();

        public static IDisposable SubscribeAsync(this IObservable<Unit> observable, Func<Task> observer)
            => observable.SelectMany(async _ =>
            {
                await observer();
                return Unit.Default;
            }).Subscribe();
    }
}
