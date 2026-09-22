namespace WebsiteQuanLyThuVien.Services.Authentication;

public interface IAuthenticationService
{
    /// <summary>
    /// Xác thực danh tính của người dùng khi họ thực hiện hành động Đăng nhập 
    /// </summary>
    /// <param name="identifier">Vừa sử dụng username và email để đăng nhập nên cái này là hợp lý</param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<AuthenticationResult?> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken = default);
}