namespace Cineaste.Data.Services;

public interface IDatabaseService
{
    void CreateDatabase(Settings settings, IEnumerable<Kind> initialKinds, IEnumerable<Tag> initialTags);
    bool ValidateDatabase();
}
