namespace Cineaste.Client.Store.ListPage;

public sealed record GoToFranchiseAction(Guid FranchiseId);

public sealed record GoToFranchiseComponentAction(Guid FranchiseId, int SequenceNumber);
