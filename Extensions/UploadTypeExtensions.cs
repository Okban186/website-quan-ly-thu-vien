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
            UploadType.BookCover => "books/covers",
            UploadType.Ebook => "digital/ebooks",
            UploadType.Audiobook => "digital/audiobooks",
            UploadType.Document => "digital/documents",
            UploadType.Excel => "imports",
            UploadType.Temp => "temp",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static string GetExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            // Images
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tiff",
            "image/svg+xml" => ".svg",

            // Documents
            "application/pdf" => ".pdf",
            "text/plain" => ".txt",
            "text/csv" => ".csv",

            // Microsoft Word
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                => ".docx",

            // Microsoft Excel
            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                => ".xlsx",

            // Microsoft PowerPoint
            "application/vnd.ms-powerpoint" => ".ppt",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation"
                => ".pptx",

            // Audio
            "audio/mpeg" => ".mp3",
            "audio/wav" => ".wav",
            "audio/x-wav" => ".wav",
            "audio/ogg" => ".ogg",
            "audio/webm" => ".weba",
            "audio/mp4" => ".m4a",
            "audio/aac" => ".aac",
            "audio/flac" => ".flac",

            // Video
            "video/mp4" => ".mp4",
            "video/webm" => ".webm",
            "video/ogg" => ".ogv",

            // Ebook
            "application/epub+zip" => ".epub",

            // Archive
            "application/zip" => ".zip",
            "application/x-7z-compressed" => ".7z",
            "application/x-rar-compressed" => ".rar",

            _ => throw new BusinessException(
                $"Không hỗ trợ loại file: {mimeType}")
        };
    }
}