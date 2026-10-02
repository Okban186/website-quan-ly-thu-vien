using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class ImportBatch
{
    public Guid Id { get; set; }

    public Guid StorageFileId { get; set; }

    public ImportBatchType ImportType { get; set; } = ImportBatchType.LIBRARY_ITEM;

    public ImportBatchStatus Status { get; set; } = ImportBatchStatus.PENDING;

    public int TotalRows { get; set; }

    public int SuccessRows { get; set; }

    public int FailedRows { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }


    // Navigation properties

    public StorageFile StorageFile { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;
}
