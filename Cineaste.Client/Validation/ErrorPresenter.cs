namespace Cineaste.Client.Validation;

using System.Text.RegularExpressions;

using Cineaste.Client.Localization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;

public sealed class ErrorPresenter : ComponentBase
{
    private object? trigger;
    private bool isTriggerUpdated = false;

    private bool isInitialized = false;
    private bool isSuspended = false;

    private string errorCode = String.Empty;
    private Regex? errorCodeRegex;

    private IEnumerable<string> currentErrors = Array.Empty<string>();

    [Parameter]
    public object? Trigger
    {
        get => this.trigger;
        set
        {
            if (!Equals(this.trigger, value))
            {
                this.trigger = value;
                this.isTriggerUpdated = true;
            }
        }
    }

    [CascadingParameter(Name = CascadingParameters.ErrorCodes)]
    public IReadOnlyCollection<string> AllErrorCodes { get; set; } = Array.Empty<string>();

    [CascadingParameter(Name = CascadingParameters.ValidationExecutor)]
    public IValidationExecutor? ValidationExecutor { get; set; }

    [Parameter]
    public string ErrorCode
    {
        get => this.errorCode;
        set
        {
            if (this.errorCode != value)
            {
                this.errorCode = value;
                this.errorCodeRegex = String.IsNullOrWhiteSpace(errorCode)
                    ? null
                    : new Regex($"^{this.errorCode.Replace(".", "\\.").Replace("*", "(.*)")}$", RegexOptions.Compiled);

                this.UpdateCurrentErrors();
            }
        }
    }

    [Parameter]
    public string Class { get; set; } = String.Empty;

    [Parameter]
    public string Style { get; set; } = String.Empty;

    [Parameter]
    public bool Popup { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Inject]
    public IStringLocalizer<Resources> Loc { get; set; } = null!;

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
