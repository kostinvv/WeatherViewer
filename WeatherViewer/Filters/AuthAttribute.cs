using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeatherViewer.Filters;

public class AuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Cookies.ContainsKey("SessionId"))
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "user", action = "login" })
            );
        }
    }
}