namespace Cineaste.Infrastructure;

using System.Text.Json;

public sealed class JsonSuspensionDriver<TAppState> : ISuspensionDriver where TAppState : class, new()
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    private readonly string file;

    public JsonSuspensionDriver(string file) =>
        this.file = file;

    public IObservable<object> LoadState()
    {
        return Observable.Return(
            JsonSerializer.Deserialize<TAppState>(File.ReadAllText(this.file), Options) ?? new());
    }

    public IObservable<Unit> SaveState(object state)
    {
        File.WriteAllText(this.file, JsonSerializer.Serialize(state, Options));
        return Observable.Return(Unit.Default);
    }

    public IObservable<Unit> InvalidateState()
    {
        if (File.Exists(this.file))
        {
            File.Delete(this.file);
        }

        return Observable.Return(Unit.Default);
    }
}
