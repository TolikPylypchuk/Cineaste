using System;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public interface IFormViewModel : IReactiveObject
    {
        IObservable<bool> FormChanged { get; }
        bool IsFormChanged { get; }

        IObservable<bool> Valid { get; }

        bool IsNew { get; }
    }
}
