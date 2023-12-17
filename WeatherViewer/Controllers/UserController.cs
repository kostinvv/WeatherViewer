using Microsoft.AspNetCore.Mvc;

namespace WeatherViewer.Controllers;

public class UserController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Registration()
    {
        return View();
    }
}