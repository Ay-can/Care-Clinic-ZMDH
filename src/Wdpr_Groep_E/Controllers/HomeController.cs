using Microsoft.AspNetCore.Mvc;

namespace Wdpr_Groep_E.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
