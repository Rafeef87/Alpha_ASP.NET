using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ProjectsController : Controller
{
    [Route("projects")]
    public IActionResult Projects()
    {
        return View();
    }
}