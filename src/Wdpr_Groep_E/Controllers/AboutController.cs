using Microsoft.AspNetCore.Mvc;

namespace Wdpr_Groep_E.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Angela_van_Heringa() => View();

        public IActionResult Gijs_Broekman() => View();

        public IActionResult Jantinus_Verduin() => View();

        public IActionResult Joseph_van_der_Vliet() => View();
    }
}
