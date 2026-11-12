using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebsiteQuanLyThuVien.Exceptions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeExceptRoleAttribute : TypeFilterAttribute
{
    public AuthorizeExceptRoleAttribute(params string[] roles) : base(typeof(AuthorizeExceptRoleFilter))
    {
        Arguments = new object[] { roles };
    }
}

public class AuthorizeExceptRoleFilter : IAuthorizationFilter
{
    private readonly string[] _excludedRoles;

    public AuthorizeExceptRoleFilter(string[] excludedRoles)
    {
        _excludedRoles = excludedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new BusinessException("Vui lòng đăng nhập để thực hiện chức năng này.", StatusCodes.Status401Unauthorized);
        }

        foreach (var role in _excludedRoles)
        {
            if (context.HttpContext.User.IsInRole(role))
            {
                throw new BusinessException("Bạn không có quyền truy cập vào tài nguyên này.", StatusCodes.Status403Forbidden);
            }
        }
    }
}

