namespace Cineaste.Client.Views.Nav;

using Microsoft.Fast.Components.FluentUI;
using Microsoft.Fast.Components.FluentUI.Utilities;

public partial class NavMenu : FluentComponentBase
{
    private const string WidthCollapsed = "40px";

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string Id { get; set; } = Identifier.NewId();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Collapsible { get; set; } = true;

    [Parameter]
    public bool Expanded { get; set; } = true;

    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    [Parameter]
    public EventCallback<bool> OnExpanded { get; set; }

    protected string? ClassValue => new CssBuilder(Class)
        .AddClass("navmenu")
        .AddClass("collapsed", !this.Expanded)
        .Build();

    protected string? StyleValue => new StyleBuilder()
        .AddStyle("width", WidthCollapsed, () => !this.Expanded)
        .AddStyle("min-width", WidthCollapsed, () => !this.Expanded)
        .AddStyle(Style)
        .Build();

    internal async Task OnExpandedToggle()
    {
        if (!this.Collapsible)
        {
            return;
        }

        this.Expanded = !Expanded;
        await this.InvokeAsync(this.StateHasChanged);

        if (this.ExpandedChanged.HasDelegate)
        {
            await this.ExpandedChanged.InvokeAsync(this.Expanded);
        }

        if (this.OnExpanded.HasDelegate)
        {
            await this.OnExpanded.InvokeAsync(this.Expanded);
        }
    }
}
