namespace Cineaste.Client.Services.Event;

using Microsoft.AspNetCore.Components.Web;

public interface IGlobalEventProvider
{
    event EventHandler<MouseEventArgs>? GlobalMouseClick;

    event EventHandler<KeyboardEventArgs>? GlobalKeyPressed;

    event EventHandler<KeyboardEventArgs>? GlobalKeyReleased;
}
