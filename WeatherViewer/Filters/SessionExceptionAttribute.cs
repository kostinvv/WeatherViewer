using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeatherViewer.Exceptions.Auth;

namespace WeatherViewer.Filters;

public class SessionExceptionAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        
        if (context.Exception is SessionNotFoundException or CookieNotFoundException)
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "user", action = "login" })
            );
            context.ExceptionHandled = true;
        }
    }
}