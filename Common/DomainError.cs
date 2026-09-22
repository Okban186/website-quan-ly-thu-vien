namespace WebsiteQuanLyThuVien.Common;

using Microsoft.AspNetCore.Http;

//Cấu hình cấu trúc một mã lỗi chuẩn gồm: Mã lỗi chuỗi, Thông điệp, và mã HTTP tương ứng
public record Error(string Code, string Message, int StatusCode);

//Nơi quản lý tập trung toàn bộ lỗi của hệ thống Thư viện

// Phân hệ Tài khoản (Account / Auth)
public static class Account
{
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Tên đăng nhập hoặc mật khẩu không chính xác.",
        StatusCodes.Status400BadRequest);

    public static readonly Error UserLocked = new(
       "Account.Locked",
       "Tài khoản của bạn đã bị khóa do vi phạm quy chế hoặc theo yêu cầu của hệ thống.",
       StatusCodes.Status403Forbidden);

    public static readonly Error UserDisabled = new(
        "Account.Disable",
        "Tài khoản bị khóa vĩnh viễn do vi phạm nghiêm trọng quy chế nội quy của thư viện",
        StatusCodes.Status403Forbidden
    );


    public static readonly Error EmailAlreadyExists = new(
        "Auth.EmailExists",
        "Địa chỉ Email này đã được đăng ký trong hệ thống.",
        StatusCodes.Status400BadRequest);

    public static readonly Error UsernameAlreadyExists = new(
        "Auth.UsernameExists",
        "Tên tài khoản này đã tồn tại.",
        StatusCodes.Status400BadRequest);

    public static readonly Error Forbidden = new(
        "Auth.Forbidden",
        "Bạn không có quyền thực hiện hành động này.",
        StatusCodes.Status403Forbidden);
}

// Phân hệ Nghiệp vụ Sách (Books)
public static class Book
{
    public static readonly Error NotFound = new(
        "Book.NotFound",
        "Cuốn sách bạn tìm kiếm không tồn tại trên hệ thống.",
        StatusCodes.Status404NotFound);

    public static readonly Error OutOfStock = new(
        "Book.OutOfStock",
        "Sách này hiện tại đã được mượn hết trong kho.",
        StatusCodes.Status400BadRequest);
}

// Phân hệ Mượn Trả (Borrow / Return)
public static class Borrow
{
    public static readonly Error MemberHasOverdueBooks = new(
        "Borrow.HasOverdue",
        "Độc giả đang có sách quá hạn chưa trả, không thể mượn thêm.",
        StatusCodes.Status400BadRequest);

    public static readonly Error ExceededLimit = new(
        "Borrow.ExceededLimit",
        "Độc giả đã mượn tối đa số lượng sách cho phép cùng một lúc.",
        StatusCodes.Status400BadRequest);
}

