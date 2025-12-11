namespace Cineaste.Application;

public abstract class TestClassBase(DbFixture db) : IAsyncLifetime
{
    protected readonly DataFixture data = db.CreateDataFixture();

    public ValueTask InitializeAsync() =>
        this.data.InitializeAsync();

    public async ValueTask DisposeAsync()
    {
        await this.data.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
