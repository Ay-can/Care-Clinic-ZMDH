using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class InfoController : Controller
    {
        private readonly ILogger<InfoController> _logger;

        public InfoController(ILogger<InfoController> logger) => _logger = logger;

        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
