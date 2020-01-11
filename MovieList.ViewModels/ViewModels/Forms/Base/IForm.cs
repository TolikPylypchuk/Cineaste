using System;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public interface IForm : IReactiveObject
    {
        IObservable<bool> FormChanged { get; }
        bool IsFormChanged { get; }

        IObservable<bool> Valid { get; }

        bool IsNew { get; }
    }
}
