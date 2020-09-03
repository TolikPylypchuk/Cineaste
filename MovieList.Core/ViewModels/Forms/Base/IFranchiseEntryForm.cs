using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels.Forms.Base
{
    public interface IFranchiseEntryForm : IReactiveObject
    {
        FranchiseEntry? FranchiseEntry { get; }
    }
}
