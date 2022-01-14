using System.Diagnostics;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;

        public AppointmentController(ILogger<AppointmentController> logger, IFluentEmail email, WdprContext context)
        {
            _logger = logger;
            _email = email;
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(string firstname, string lastname, string infix, string email, string phone, string subject, string message)
        {
            _context.Add(new AppointmentModel()
            {
                FirstName = firstname,
                LastName = lastname,
                Infix = infix,
                PhoneNumber = phone,
                Subject = subject,
                Message = message,
                Email = email
            });
            _context.SaveChanges();

            var sender = _email
                .To(email)
                .Subject("Afspraak succesvol aangevraagd")
                .Body($"Uw afspraak voor een intake gesprek over {subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");

            await sender.SendAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
