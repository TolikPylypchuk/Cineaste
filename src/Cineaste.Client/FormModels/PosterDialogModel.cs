namespace Cineaste.Client.FormModels;

public enum PosterSelectionMode
{
    File, Url
}

public class PosterDialogModel
{
    public const long MaxFileSize = 10_000_000;
    public const string FileTooLargeError = "Poster.File.TooLarge";

    private PosterSelectionMode selectionMode = PosterSelectionMode.File;
    private IBrowserFile? selectedFile;
    private string posterUrl = String.Empty;
    private PosterUrlRequest? urlRequest;

    public PosterSelectionMode SelectionMode
    {
        get => this.selectionMode;
        set
        {
            this.selectionMode = value;
            this.SelectedFile = null;
            this.PosterUrl = String.Empty;
        }
    }

    public IBrowserFile? SelectedFile
    {
        get => this.selectedFile;
        set
        {
            this.selectedFile = value;
            this.ValidationErrors = [];

            if (value is not null && value.Size > MaxFileSize)
            {
                this.ValidationErrors = this.ValidationErrors.Add(FileTooLargeError);
            }
        }
    }

    public string PosterUrl
    {
        get => this.posterUrl;
        set
        {
            this.posterUrl = value;
            this.ValidationErrors = [];

            var request = new PosterUrlRequest(value);

            var result = PosterUrlRequest.Validator.Validate(request);

            if (result.IsValid)
            {
                this.urlRequest = request;
            } else
            {
                var errors = this.ValidationErrors.ToBuilder();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.ErrorCode);
                }

                this.ValidationErrors = errors.ToImmutable();
            }
        }
    }

    public ImmutableHashSet<string> ValidationErrors { get; private set; } = [];

    public bool IsValid =>
        this.ValidationErrors.Count == 0 &&
            this.SelectionMode switch
            {
                PosterSelectionMode.File => this.SelectedFile is not null,
                PosterSelectionMode.Url => !String.IsNullOrWhiteSpace(this.PosterUrl),
                _ => false
            };

    public PosterRequest? CreateRequest()
    {
        if (this.ValidationErrors.Count != 0)
        {
            return null;
        }

        return this.SelectionMode switch
        {
            PosterSelectionMode.File => this.SelectedFile is not null ? new PosterRequest(this.SelectedFile) : null,
            PosterSelectionMode.Url => this.urlRequest is not null ? new PosterRequest(this.urlRequest) : null,
            _ => null
        };
    }
}
