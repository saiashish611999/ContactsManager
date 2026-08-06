using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
public sealed class PersonsController: Controller
{
    [Route("/")]
    [Route("[action]")]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}
