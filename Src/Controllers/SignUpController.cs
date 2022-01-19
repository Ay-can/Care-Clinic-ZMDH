using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class SignUpController : Controller
    {
        private readonly ILogger<SignUpController> _logger;
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SignUpController(ILogger<SignUpController> logger, IFluentEmail email, WdprContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _email = email;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public IActionResult Client() => View(_context.Subjects);

        public IActionResult Child() => View(_context.Subjects);

        [HttpPost]
        public async Task<IActionResult> CreateSignUp(string firstname, string lastname, string infix, string email, string phone, string subject, string message)
        {
            _context.Add(new SignUp()
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

        [HttpGet]
        [Authorize(Roles = "Orthopedagoog")]
        public IActionResult Overview()
        {
            return View(_context.SignUps.ToList());
        }

        [HttpPost]
        public IActionResult AcceptSignUp(string firstname, string email, int Id, string Subject)
        {
            var user = new AppUser { UserName = firstname + "69", Email = email };
            var result = _userManager.CreateAsync(user, "Test123!");
            DeleteSignUp(Id);
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        public IActionResult DeleteSignUp(int id)
        {
            var getSignUp = _context.SignUps.Single(s => s.Id == id);

            _context.SignUps.Remove(getSignUp);
            _context.SaveChanges();
            return RedirectToAction("Overview");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
