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
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    public class SignUpController : Controller
    {
        private readonly ILogger<SignUpController> _logger;
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IZmdhApi _api;

        public SignUpController(ILogger<SignUpController> logger, IFluentEmail email, WdprContext context, UserManager<AppUser> userManager,
        IZmdhApi api)
        {
            _logger = logger;
            _email = email;
            _context = context;
            _userManager = userManager;
            _api = api;
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
            var UniqueId = int.Parse(_api.CreateClientId().Result);
            var UniqueChildId = UniqueId + 1;
            System.Console.WriteLine(UniqueId);
            System.Console.WriteLine(UniqueChildId);

            SignUp CreateSignUp = new SignUp();

            CreateSignUp.FirstName = firstname;
            CreateSignUp.LastName = lastname;
            CreateSignUp.Infix = infix;
            CreateSignUp.Email = email;
            CreateSignUp.PhoneNumber = phone;
            CreateSignUp.Subject = subject;
            CreateSignUp.Message = message;
            CreateSignUp.UserName = firstname + lastname;
         

            CreateSignUp.Children = new Collection<SignUpChild>() {new SignUpChild() {
                   Username = childFirstname,
                   FirstName = childFirstname,
                   Infix = childInfix,
                   LastName = childLastname,
                   Subject = subject,
                   }};

            _context.Add(CreateSignUp);
            await _context.SaveChangesAsync();

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
            _context.SaveChangesAsync();
            return View(_context.SignUps.Include(s => s.Children).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSignUpWithChildren(string firstname, string infix, string lastname, string email, string Subject, string childFirstname, string childInfix, string childLastname, string childUsername, DateTime birthdate, string phone, int Id, string childId)
        {


            var child = new AppUser() { UserName = childUsername, FirstName = childFirstname, LastName = childLastname, Infix = childInfix, Email = "", Subject = Subject, BirthDate = birthdate };
            _context.SaveChanges();
            var user = new AppUser { UserName = email, Email = email, Children = new Collection<AppUser>() { child }, FirstName = firstname, LastName = lastname, Infix = infix, Subject = Subject, PhoneNumber = phone, };

            var createUser = _userManager.CreateAsync(user, "Test123!");
            var createChildUser = _userManager.CreateAsync(child, "Test123!");
            _context.ChangeTracker.Clear();
            await _context.SaveChangesAsync();
            await DeleteSignUp(Id);
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSignUpAsync(int Id, string username, DateTime birthdate, string firstname, string infix, string lastname, string email, string phone, string subject)
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
            await _context.SaveChangesAsync();
            await DeleteSignUp(Id);
            return RedirectToAction("Overview", "SignUp");

        }

        [HttpPost]
        public  async Task<IActionResult> DeleteSignUp(int id)
        {
            var getSignUp = _context.SignUps.Single(s => s.Id == id);
            _context.Remove(getSignUp);
           await _context.SaveChangesAsync();
            return RedirectToAction("Overview");
        }
        // [HttpPost] IActionResult DeleteSignUpWithChildren(int childId)
        // {
        //     var getChildSignUp = _context.SignUpChildren.Single(s => s.Id == childId);
        //     _context.SignUpChildren.Remove(getChildSignUp);
        //     _context.SaveChanges();
        //     return RedirectToAction("Overview");
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
