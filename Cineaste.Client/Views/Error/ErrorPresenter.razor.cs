namespace Cineaste.Client.Views.Error;

using System.Text.RegularExpressions;

using Microsoft.Extensions.Localization;

public sealed partial class ErrorPresenter : ComponentBase
{
    private bool isTriggerUpdated = false;
    private bool isInitialized = false;
    private bool isSuspended = false;
    private Regex? errorCodeRegex;

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

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = [];

    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    private IEnumerable<string> CurrentErrors { get; set; } = Array.Empty<string>();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(this.Trigger), out object? trigger) && trigger != this.Trigger)
        {
            this.isTriggerUpdated = true;
        }

        if (parameters.TryGetValue(nameof(this.ErrorCode), out string? errorCode) && errorCode != this.ErrorCode)
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
            this.ValidationExecutor.ValidationCleared += (s, e) => this.CurrentErrors = Array.Empty<string>();
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

    private void UpdateCurrentErrors()
    {
        if (this.isInitialized && !this.isSuspended && this.errorCodeRegex is not null)
        {
            this.CurrentErrors = this.AllErrorCodes
                .Distinct()
                .Where(errorCode => this.errorCodeRegex.IsMatch(errorCode))
                .ToList();
        }
    }
}
