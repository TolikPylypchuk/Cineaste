using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.ViewModels.Forms.Base
{
    public interface IFranchiseEntryForm : IReactiveObject
    {
        FranchiseEntry? FranchiseEntry { get; }
    }
}
