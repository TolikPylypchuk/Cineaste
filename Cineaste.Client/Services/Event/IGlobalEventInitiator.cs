namespace Cineaste.Client.Services.Event;

using Microsoft.AspNetCore.Components.Web;

public interface IGlobalEventInitiator
{
    void OnGlobalMouseClick(MouseEventArgs e);

    void OnGlobalKeyPressed(KeyboardEventArgs e);

    void OnGlobalKeyReleased(KeyboardEventArgs e);
}
