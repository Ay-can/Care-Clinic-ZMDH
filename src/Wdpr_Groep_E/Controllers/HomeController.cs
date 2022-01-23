using Microsoft.AspNetCore.Mvc;

namespace Wdpr_Groep_E.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Error() => RedirectToAction("Index", "Message", new
        {
            Type = "Error",
            Message = "Er is een fout opgetreden, u wordt terug gestuurd naar de homepagina.",
            Redirect = "Home",
            Timeout = 3000
        });
    }
}
