using System;
using backendProject.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using P0_ClassLibrary.Interfaces;

namespace backendProject.Middlewares;

[AttributeUsage(AttributeTargets.All)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Cookies["Backend_Auth_Token"];
        if (token == null)
        {
            context.Result = new JsonResult(new { message = "Session Expired ! Kindly Login Again ..." })
            {
                StatusCode = 401
            };
            return;
        }
        var tokenservice = context.HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;
        var userId = tokenservice?.VerifyTokenAndGetId(token);
        if (userId == Guid.Empty)
        {
            context.Result = new JsonResult(new { message = "Forbidden ! Cannot Access The Resources ..." })
            {
                StatusCode = 403
            };
            return;
        }
        else
        {
            context.HttpContext.Items["userId"] = userId;
        }

    }
}
