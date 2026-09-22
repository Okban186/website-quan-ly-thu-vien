using Microsoft.AspNetCore.Identity;
using WebsiteQuanLyThuVien.Common;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories;

namespace WebsiteQuanLyThuVien.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{

    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IRefreshTokenService _refreshTokenService;

    public AuthenticationService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordHasher<User> passwordHasher, IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;

    }
    public async Task<AuthenticationResult?> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameOrEmailAsync(identifier);

        if (user == null)
            return null;

        if (String.Equals(user.Status, "LOCKED", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException(Account.UserLocked.Message, Account.UserLocked.StatusCode);

        if (String.Equals(user.Status, "DISABLE", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException(Account.UserDisabled.Message, Account.UserDisabled.StatusCode);

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordResult == PasswordVerificationResult.Failed)
            throw new BusinessException(Account.InvalidCredentials.Message, Account.InvalidCredentials.StatusCode);

        var roles = await _userRepository.GetRolesAsync(user.Id);

        var access_token = _jwtTokenService.GenerateToken(user, roles);

        var refresh_token = await _refreshTokenService.CreateAsync(user.Id);

        await _userRepository.UpdateLastLoginAsync(user.Id);

        return new AuthenticationResult
        {
            AccessToken = access_token,
            RefreshToken = refresh_token
        };
    }


}