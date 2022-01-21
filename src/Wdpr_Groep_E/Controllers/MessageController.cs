using Microsoft.AspNetCore.Mvc;

namespace Wdpr_Groep_E.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index(string Message, string Type, string Redirect, int Timeout)
        {
            ViewData["Type"] = Type;
            ViewData["Message"] = Message;
            ViewData["Redirect"] = Redirect;
            ViewData["Timeout"] = Timeout;
            return View();
        }
    }
}
