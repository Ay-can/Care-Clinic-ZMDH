using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public IActionResult Client() => View();

        public IActionResult Child() => View();

        [HttpPost]
        public async Task<IActionResult> CreateSignUp(string firstname, string lastname, string infix, string email, string phone, string subject, string message, DateTime birthdate, string username)
        {
            _context.Add(new SignUp()
            {
                FirstName = firstname,
                LastName = lastname,
                Infix = infix,
                PhoneNumber = phone,
                Subject = subject,
                Message = message,
                Email = email,
                BirthDate = birthdate,
                UserName = username

            });
            _context.SaveChanges();

            var sender = _email
                .To(email)
                .Subject("Aanmelding  account succesvol aangevraagd")
                .Body($"Uw aanmelding voor een zmdh account over {subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");

            await sender.SendAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSignUpWithChild(string firstname, string lastname, string infix, string email, string phone, string subject, string message, string childUsername, string childFirstname, string childInfix, string childLastname)
        {
            _context.Add(new SignUp()
            {
                FirstName = firstname,
                LastName = lastname,
                Infix = infix,
                Email = email,
                PhoneNumber = phone,
                Subject = subject,
                Message = message,
                UserName = firstname + lastname,
                Children = new Collection<SignUpChild>() {new SignUpChild() {
                   Username = childFirstname,
                   FirstName = childFirstname,
                   Infix = childInfix,
                   LastName = childLastname,
                   Subject = subject
               }}
            });
            _context.SaveChanges();

            var sender = _email
                .To(email)
                .Subject("Aanmelding ouder en kind succesvol aangevraagd")
                .Body($"Uw aanmelding voor een zmdh account over {subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");

            await sender.SendAsync();

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        [Authorize(Roles = "Orthopedagoog")]
        public IActionResult Overview()
        {
            return View(_context.SignUps.Include(s => s.Children).ToList());
        }

        [HttpPost]
        public IActionResult AcceptSignUpWithChildren(string firstname, string infix, string lastname, string email, int Id, string Subject, string childFirstname, string childInfix, string childLastname, string childUsername, int childId, DateTime birthdate, string phone)
        {
            var child = new AppUser() { UserName = childUsername, FirstName = childFirstname, LastName = childLastname, Infix = childInfix, Email = email, Subject = Subject, BirthDate = birthdate};
            var user = new AppUser { UserName = email, Email = email, Children = new Collection<AppUser>() { child }, FirstName = firstname, LastName = lastname, Infix = infix , Subject = Subject,  PhoneNumber = phone};

            var createUser = _userManager.CreateAsync(user, "Test123!");
            var createChildUser = _userManager.CreateAsync(child, "Test123!");
             DeleteSignUp(Id);
            DeleteSignUpWithChildren(childId);
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        public IActionResult AcceptSignUp(int Id, string username, DateTime birthdate, string firstname, string infix, string lastname, string email, string phone, string subject)
        {
            var user = new AppUser
            {
                UserName = username,
                BirthDate = birthdate,
                FirstName = firstname,
                Infix = infix,
                LastName = lastname,
                Email = email,
                PhoneNumber = phone,
                Subject = subject,
            };
            var createUser = _userManager.CreateAsync(user, "Test123!");
            //role toevoegen
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
        [HttpPost] IActionResult DeleteSignUpWithChildren(int childId)
        {
            var getChildSignUp = _context.SignUpChildren.Single(s => s.Id == childId);
            _context.SignUpChildren.Remove(getChildSignUp);
            _context.SaveChanges();
            return RedirectToAction("Overview");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
