using Cineaste.Data.Models;

using ReactiveUI;

namespace Cineaste.Core.ViewModels.Forms.Base
{
    public interface IFranchiseEntryForm : IReactiveObject
    {
        FranchiseEntry? FranchiseEntry { get; }
    }
}
