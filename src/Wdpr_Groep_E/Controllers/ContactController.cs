using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Wdpr_Groep_E.Data;

namespace Wdpr_Groep_E.Controllers
{
    public class ContactController : Controller
    {
        private readonly WdprContext _context;

        public ContactController(WdprContext context) => _context = context;

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Contact([FromServices] IFluentEmailFactory emailFactory, string email, string subject, string message)
        {
            var contact = emailFactory.Create()
                .To(email)
                .Subject("Bedankt voor uw bericht!")
                .Body($"Bedankt voor uw bericht! We zullen zo snel mogelijk contact met u opnemen.");
            await contact.SendAsync();

            var sender = emailFactory.Create()
                .To("zmdh.hulp@gmail.com")
                .Subject("Contact")
                .Body($"{email} heeft een bericht gestuurd met de onderwerp {subject} en het bericht {message}");
            await sender.SendAsync();

            return RedirectToAction("Index", "Message", new
            {
                Type = "Success",
                Message = "Uw bericht is successvol verzonden.",
                Redirect = "Home",
                Timeout = 2000
            });
        }
    }
}
