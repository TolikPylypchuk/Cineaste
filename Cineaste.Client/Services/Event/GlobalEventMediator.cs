namespace Cineaste.Client.Services.Event;

using Microsoft.AspNetCore.Components.Web;

public sealed class GlobalEventMediator : IGlobalEventInitiator, IGlobalEventProvider
{
    public void OnGlobalMouseClick(MouseEventArgs e) =>
        this.GlobalMouseClick?.Invoke(this, e);

    public void OnGlobalKeyPressed(KeyboardEventArgs e) =>
        this.GlobalKeyPressed?.Invoke(this, e);

    public void OnGlobalKeyReleased(KeyboardEventArgs e) =>
        this.GlobalKeyReleased?.Invoke(this, e);

    public event EventHandler<MouseEventArgs>? GlobalMouseClick;

    public event EventHandler<KeyboardEventArgs>? GlobalKeyPressed;

    public event EventHandler<KeyboardEventArgs>? GlobalKeyReleased;
}
