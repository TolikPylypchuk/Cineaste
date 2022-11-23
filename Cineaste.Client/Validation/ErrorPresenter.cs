namespace Cineaste.Client.Validation;

using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Cineaste.Client.Localization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;

using Radzen;

public sealed class ErrorPresenter : ComponentBase
{
    private bool isTriggerUpdated = false;
    private bool isInitialized = false;
    private bool isSuspended = false;
    private Regex? errorCodeRegex;
    private IEnumerable<string> currentErrors = Array.Empty<string>();

    [Parameter]
    public object? Trigger { get; set; }

    [CascadingParameter(Name = CascadingParameters.ErrorCodes)]
    public IReadOnlyCollection<string> AllErrorCodes { get; set; } = Array.Empty<string>();

    [CascadingParameter(Name = CascadingParameters.ValidationExecutor)]
    public IValidationExecutor? ValidationExecutor { get; set; }

    [Parameter]
    public string? ErrorCode { get; set; }

    [Parameter]
    public string Class { get; set; } = String.Empty;

    [Parameter]
    public string Style { get; set; } = String.Empty;

    [Parameter]
    public bool Popup { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.DidParameterChange(nameof(this.Trigger), this.Trigger))
        {
            this.isTriggerUpdated = true;
        }

        if (parameters.DidParameterChange(nameof(this.ErrorCode), this.ErrorCode) &&
            parameters.TryGetValue(nameof(this.ErrorCode), out string? errorCode))
        {
            this.errorCodeRegex = String.IsNullOrWhiteSpace(errorCode)
                ? null
                : new Regex($"^{errorCode.Replace(".", "\\.").Replace("*", "(.*)")}$", RegexOptions.Compiled);

            this.UpdateCurrentErrors();
        }

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!this.isInitialized && this.ValidationExecutor is not null)
        {
            this.ValidationExecutor.ValidationExecuted += (s, e) => this.UpdateCurrentErrors();
            this.ValidationExecutor.ValidationCleared += (s, e) => this.currentErrors = Array.Empty<string>();
            this.ValidationExecutor.ValidationSuspended += (s, e) => this.isSuspended = true;
            this.ValidationExecutor.ValidationResumed += (s, e) => this.isSuspended = false;
        }

        if (this.isTriggerUpdated)
        {
            this.UpdateCurrentErrors();
            this.isTriggerUpdated = false;
        }

        this.isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        foreach (string errorCode in this.currentErrors)
        {
            var @class = $"rz-message rz-messages-error {(this.Popup ? "rz-message-popup" : "")} {this.Class}";

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", this.Style);
            builder.AddAttribute(2, "class", @class);
            builder.AddMultipleAttributes(3, this.Attributes);
            builder.AddContent(4, this.Loc[$"Validation.{errorCode}"]);
            builder.CloseElement();
        }
    }

    private void UpdateCurrentErrors()
    {
        if (this.isInitialized && !this.isSuspended && this.errorCodeRegex is not null)
        {
            this.currentErrors = this.AllErrorCodes
                .Distinct()
                .Where(errorCode => this.errorCodeRegex.IsMatch(errorCode))
                .ToList();
        }
    }
}
