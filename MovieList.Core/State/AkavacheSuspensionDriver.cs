using System;
using System.Reactive;

using Akavache;

using ReactiveUI;

namespace MovieList.State
{
    public sealed class AkavacheSuspensionDriver<TAppState> : ISuspensionDriver where TAppState : class
    {
        private const string AppStateKey = "appState";

        public IObservable<Unit> InvalidateState()
            => BlobCache.UserAccount.InvalidateObject<TAppState>(AppStateKey);

        public IObservable<object> LoadState()
            => BlobCache.UserAccount.GetObject<TAppState>(AppStateKey);

        public IObservable<Unit> SaveState(object state)
            => BlobCache.UserAccount.InsertObject(AppStateKey, (TAppState)state);
    }
}
