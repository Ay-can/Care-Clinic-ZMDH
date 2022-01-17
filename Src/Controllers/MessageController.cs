using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;

        public MessageController(ILogger<MessageController> logger) => _logger = logger;

        public IActionResult Index(string Message, string Type, string Redirect, int Timeout)
        {
            ViewData["Type"] = Type;
            ViewData["Message"] = Message;
            ViewData["Redirect"] = Redirect;
            ViewData["Timeout"] = Timeout;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
