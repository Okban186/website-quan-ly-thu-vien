using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

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
            context.Result = new UnauthorizedResult();
            return;
        }


        foreach (var role in _excludedRoles)
        {
            if (context.HttpContext.User.IsInRole(role))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
