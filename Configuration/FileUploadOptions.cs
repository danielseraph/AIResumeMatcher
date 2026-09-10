namespace AIResumeMatcher.Configuration;

public sealed class FileUploadOptions
{
    /// <summary>Maximum allowed file size in bytes. Default: 5 MB.</summary>
    public long MaxFileSizeBytes { get; set; } = 5_242_880;

    /// <summary>MIME types accepted for upload.</summary>
    public string[] AllowedContentTypes { get; set; } = ["application/pdf"];
}
