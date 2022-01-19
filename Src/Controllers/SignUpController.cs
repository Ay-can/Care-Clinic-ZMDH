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

        // [HttpPost]
        // public async Task<IActionResult> CreateSignUp(string firstname, string lastname, string infix, string email, string phone, string subject, string message, DateTime birthdate, string username)
        // {
        //     _context.Add(new SignUp()
        //     {
        //         FirstName = firstname,
        //         LastName = lastname,
        //         Infix = infix,
        //         PhoneNumber = phone,
        //         Subject = subject,
        //         Message = message,
        //         Email = email,
        //         BirthDate = birthdate,
        //         UserName = username
        //     });
        //     _context.SaveChanges();

        //     var sender = _email
        //         .To(email)
        //         .Subject("Aanmelding  account succesvol aangevraagd")
        //         .Body($"Uw aanmelding voor een zmdh account over {subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");

        //     await sender.SendAsync();
        //     return RedirectToAction("Index", "Home");
        // }
         [HttpPost]
        public async Task<IActionResult> CreateSignUpV2(SignUp s)
        {
            _context.Add(new SignUp()
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
                Infix = s.Infix,
                PhoneNumber = s.PhoneNumber,
                Subject = s.Subject,
                Message = s.Message,
                Email = s.Email,
                BirthDate = s.BirthDate,
                UserName = s.UserName
            });
             _context.SaveChanges();

            var sender = _email
                .To(s.Email)
                .Subject("Aanmelding  account succesvol aangevraagd")
                .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");
             await sender.SendAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSignUpWithChildV2(SignUp s, SignUpChild c)
        {
               
            _context.Add(new SignUp(){
                FirstName = s.FirstName,
                LastName = s.LastName,
                Infix = s.Infix,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                  Subject = s.Subject,
                Message = s.Message,
                UserName = s.Email,
                Children = new Collection<SignUpChild>() {
                   new SignUpChild() {
                       ChildUserName = c.ChildUserName,
                       ChildFirstName = c.ChildFirstName,
                       ChildLastName = c.ChildLastName,
                       ChildInfix = c.ChildInfix,
                       ChildBirthDate = c.ChildBirthDate,
                       Subject = c.Subject
                   }
                }
            });
            
            await _context.SaveChangesAsync();
            var sender = _email
                .To(s.Email)
                .Subject("Aanmelding ouder en kind succesvol aangevraagd")
                .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");

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
            //await DeleteSignUp(Id);
            return RedirectToAction("Overview", "SignUp");
        }
         [HttpPost]
        public async Task<IActionResult> AcceptSignUpWithChildrenV2(SignUp s, SignUpChild c)
        {
            
                
            var child = new AppUser() { UserName = c.ChildUserName, FirstName = c.ChildFirstName, LastName = c.ChildLastName, Infix = c.ChildInfix, Email = s.Email, Subject = c.Subject, BirthDate = c.ChildBirthDate};
            _context.SaveChanges();
            var user = new AppUser { UserName = s.FirstName + s.LastName, Email = s.Email, Children = new Collection<AppUser>() { child }, FirstName = s.FirstName, LastName = s.LastName, Infix = s.Infix, Subject = s.Subject, PhoneNumber = s.PhoneNumber};

            var createUser = _userManager.CreateAsync(user, "Test123!");
            var createChildUser = _userManager.CreateAsync(child, "Test123!");
            _context.ChangeTracker.Clear();
            await _context.SaveChangesAsync();
            await DeleteSignUp(s.TempId);
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
           // await DeleteSignUp(s.FirstName,s.LastName,s.BirthDate,s.Email);

           // await DeleteSignUp(Id);
            return RedirectToAction("Overview", "SignUp");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteSignUp(string Id)
        {
            var getSignUp = _context.SignUps.SingleOrDefault(s => s.TempId == Id);
            _context.Remove(getSignUp);
            await _context.SaveChangesAsync();
            return RedirectToAction("Overview");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
