using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Extensions;

public static class UploadTypeExtensions
{
    public static string GetFinalPrefix(this UploadType type)
    {
        return type switch
        {
            UploadType.Avatar => "members/avatars",
            UploadType.IdentityFront => "members/identity/front",
            UploadType.IdentityBack => "members/identity/back",
            UploadType.ResourceImage => "books/covers",
            UploadType.Ebook => "digital/ebooks",
            UploadType.Audiobook => "digital/audiobooks",
            UploadType.Document => "digital/documents",
            UploadType.Excel => "imports",
            UploadType.Temp => "temp",

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static long GetMaxFileSize(this UploadType type)
    {
        return type switch
        {
            UploadType.Avatar => 2 * 1024 * 1024,
            UploadType.IdentityFront => 5 * 1024 * 1024,
            UploadType.IdentityBack => 5 * 1024 * 1024,
            UploadType.ResourceImage => 10 * 1024 * 1024,
            UploadType.Ebook => 50 * 1024 * 1024,
            UploadType.Audiobook => 200 * 1024 * 1024,
            UploadType.Document => 50 * 1024 * 1024,
            UploadType.Excel => 20 * 1024 * 1024,
            UploadType.Temp => 50 * 1024 * 1024,

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static IReadOnlySet<string> GetAllowedMimeTypes(this UploadType type)
    {
        return type switch
        {
            UploadType.Avatar or
            UploadType.IdentityFront or
            UploadType.IdentityBack or
            UploadType.ResourceImage => new HashSet<string>(
                    [
                        "image/jpeg",
                        "image/png",
                        "image/webp"
                    ],
                    StringComparer.OrdinalIgnoreCase),

            UploadType.Ebook => new HashSet<string>(
                    [
                        "application/epub+zip"
                    ],
                    StringComparer.OrdinalIgnoreCase),

            UploadType.Audiobook => new HashSet<string>(
                    [
                        "audio/mpeg",
                        "audio/mp4",
                        "audio/ogg",
                        "audio/wav",
                        "audio/flac"
                    ],
                    StringComparer.OrdinalIgnoreCase),

            UploadType.Document => new HashSet<string>(
                    [
                        "application/pdf",
                        "text/plain",
                        "application/msword",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    ],
                    StringComparer.OrdinalIgnoreCase),

            UploadType.Excel => new HashSet<string>(
                    [
                        "application/vnd.ms-excel",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "text/csv"
                    ],
                    StringComparer.OrdinalIgnoreCase),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string GetExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            // Images
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",

            // Ebook
            "application/epub+zip" => ".epub",

            // Audio
            "audio/mpeg" => ".mp3",
            "audio/mp4" => ".m4a",
            "audio/ogg" => ".ogg",
            "audio/wav" => ".wav",
            "audio/flac" => ".flac",

            // Documents
            "application/pdf" => ".pdf",
            "text/plain" => ".txt",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                => ".docx",

            // Excel
            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                => ".xlsx",
            "text/csv" => ".csv",

            _ => throw new BusinessException(
                $"Không hỗ trợ MIME type: {mimeType}")
        };
    }
}