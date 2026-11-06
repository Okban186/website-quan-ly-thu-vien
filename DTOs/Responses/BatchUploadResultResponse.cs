public record BatchItemResult(Guid UploadSessionId, bool IsSuccess, int StatusCode, string? ErrorMessage, Guid? StorageFileId);

public class BatchUploadResultResponse
{
    public List<BatchItemResult> Successful { get; set; } = new();
    public List<BatchItemResult> Failed { get; set; } = new();
}