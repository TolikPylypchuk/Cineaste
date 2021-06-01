using System;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public interface IReactiveForm : IReactiveObject
    {
        IObservable<bool> FormChanged { get; }
        bool IsFormChanged { get; }

        IObservable<bool> Valid { get; }

        bool IsNew { get; }
    }
}
