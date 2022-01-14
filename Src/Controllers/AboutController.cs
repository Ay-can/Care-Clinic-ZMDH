using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;

        public AboutController(ILogger<AboutController> logger) => _logger = logger;

        public IActionResult Index() => View();

        public IActionResult Angela_van_Heringa() => View();

        public IActionResult Gijs_Broekman() => View();

        public IActionResult Jantinus_Verduin() => View();

        public IActionResult Joseph_van_der_Vliet() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
