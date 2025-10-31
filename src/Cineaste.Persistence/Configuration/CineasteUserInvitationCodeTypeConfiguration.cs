using Cineaste.Identity;

namespace Cineaste.Persistence.Configuration;

internal sealed class CineasteUserInvitationCodeTypeConfiguration : IEntityTypeConfiguration<CineasteUserInvitationCode>
{
    public void Configure(EntityTypeBuilder<CineasteUserInvitationCode> invitationCode) =>
        invitationCode.HasKey(l => l.Id);
}
