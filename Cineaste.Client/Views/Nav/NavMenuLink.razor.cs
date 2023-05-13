namespace Cineaste.Client.Views.Nav;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.Fast.Components.FluentUI.Utilities;

public partial class NavMenuLink : FluentComponentBase
{
    [Parameter]
    public string Icon { get; set; } = String.Empty;

    [Parameter]
    public string Id { get; set; } = Identifier.NewId();

    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter]
    public bool Selected { get; set; } = false;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public string Text { get; set; } = String.Empty;

    [CascadingParameter(Name = nameof(NavMenuExpanded))]
    public bool NavMenuExpanded { get; set; }

    private string? ClassValue => new CssBuilder(this.Class)
        .AddClass("navmenu-link")
        .Build();

    internal void OnIconClick()
    {
        if (!this.Disabled)
        {
            this.Selected = true;
        }
    }
}
