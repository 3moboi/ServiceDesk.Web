using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Web.Controllers;

public class ErrorController : Controller
{
    [Route("Error/Error")]
    public IActionResult Error()
    {
        return View();
    }
}