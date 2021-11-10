namespace Cineaste.Core.ViewModels.Forms.Preferences;

public sealed class PreferencesFormViewModel : SettingsFormBase<UserPreferences, PreferencesFormViewModel>
{
    private readonly IBlobCache store;

    public PreferencesFormViewModel(
        UserPreferences userPreferences,
        IBlobCache? store = null,
        ThemeManager? themeManager = null,
        ResourceManager? resourceManager = null,
        IScheduler? scheduler = null)
        : base(userPreferences, resourceManager, scheduler)
    {
        this.store = store ?? GetDefaultService<IBlobCache>(StoreKey);
        themeManager ??= GetDefaultService<ThemeManager>();

        this.Header = new TabHeaderViewModel(
            String.Empty, this.ResourceManager.GetString("Preferences") ?? String.Empty);

        this.CopyProperties();

        this.WhenAnyValue(vm => vm.Theme)
            .ObserveOn(RxApp.MainThreadScheduler)
            .BindTo(themeManager, tm => tm.Theme);

        this.CanNeverDelete();
        this.EnableChangeTracking();
    }

    [Reactive]
    public Theme Theme { get; set; }

    [Reactive]
    public bool ShowRecentFiles { get; set; }

    [Reactive]
    public string LogPath { get; set; } = String.Empty;

    [Reactive]
    public int MinLogLevel { get; set; }

    public TabHeaderViewModel Header { get; }

    protected override PreferencesFormViewModel Self => this;

    protected override void EnableChangeTracking()
    {
        this.TrackChanges(vm => vm.Theme, vm => vm.Model.UI.Theme);
        this.TrackChanges(vm => vm.ShowRecentFiles, vm => vm.Model.File.ShowRecentFiles);
        this.TrackChanges(vm => vm.LogPath, vm => vm.Model.Logging.LogPath);
        this.TrackChanges(vm => vm.MinLogLevel, vm => vm.Model.Logging.MinLogLevel);

        base.EnableChangeTracking();
    }

    protected override IObservable<UserPreferences> OnSave()
    {
        this.Model.UI.Theme = this.Theme;
        this.Model.File.ShowRecentFiles = this.ShowRecentFiles;

        if (!this.Model.File.ShowRecentFiles)
        {
            this.Model.File.RecentFiles.Clear();
        }

        this.Model.Logging.LogPath = this.LogPath;
        this.Model.Logging.MinLogLevel = this.MinLogLevel;

        return base.OnSave()
            .DoAsync(_ => this.store.InsertObject(PreferencesKey, this.Model).Eager());
    }

    protected override void CopyProperties()
    {
        this.Theme = this.Model.UI.Theme;
        this.ShowRecentFiles = this.Model.File.ShowRecentFiles;
        this.LogPath = this.Model.Logging.LogPath;
        this.MinLogLevel = this.Model.Logging.MinLogLevel;

        base.CopyProperties();
    }

    protected override bool InitialKindIsNewValue(Kind kind) =>
        !this.Model.Defaults.DefaultKinds.Contains(kind);

    protected override bool IsTagNew(TagModel tagModel) =>
        false;
}
