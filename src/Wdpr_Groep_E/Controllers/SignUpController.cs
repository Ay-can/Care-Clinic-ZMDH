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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IZmdhApi _api;

        public SignUpController(ILogger<SignUpController> logger, IFluentEmail email, WdprContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IZmdhApi api)
        {
            _logger = logger;
            _email = email;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _api = api;
        }

        public IActionResult Index() => View();

        public IActionResult Client() => View(_context.Subjects);

        public IActionResult Child() => View(_context.Subjects);

        [HttpGet]
        [Authorize(Roles = "Orthopedagoog")]
        public IActionResult Overview()
        {
            _context.SaveChangesAsync();
            return View(_context.SignUps.Include(s => s.Children).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignUp(SignUp s, string careGiver)
        {
            if (!_userManager.Users.Any(u => u.Email == s.Email))
            {
                if (!_userManager.Users.Any(u => u.UserName == s.UserName))
                {
                    if (!(s.BirthDate > DateTime.Now.AddYears(-16)))
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
                            UserName = s.UserName,
                            CareGiver = careGiver
                        });
                        await _context.SaveChangesAsync();
                        var sender = _email
                            .To(s.Email)
                            .Subject("Aanmelding  account succesvol aangevraagd")
                            .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");
                        await sender.SendAsync();
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Success",
                            Message = "Uw aanmelding is successvol verzonden.",
                            Redirect = "Home",
                            Timeout = 2500
                        });
                    }
                    else
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Failed",
                            Message = "U moet ouder dan 16 zijn om u aan te melden, laat anders je ouders/verzorgers je aanmelden.",
                            Redirect = "SignUp/Client",
                            Timeout = 5000
                        });
                }
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Er bestaat al een account met deze gebruikersnaam.",
                        Redirect = "SignUp/Client",
                        Timeout = 2500
                    });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Er bestaat al een account met dit e-mailadres.",
                    Redirect = "SignUp/Client",
                    Timeout = 2500
                });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignUpWithChild(SignUp s, SignUpChild c, string careGiver)
        {
            if (!_userManager.Users.Any(u => u.Email == s.Email))
            {
                if (!_userManager.Users.Any(u => u.UserName == s.UserName))
                {
                    if (!(s.BirthDate < DateTime.Now.AddYears(-16)))
                    {
                        _context.Add(new SignUp()
                        {
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            Infix = s.Infix,
                            Email = s.Email,
                            PhoneNumber = s.PhoneNumber,
                            Subject = s.Subject,
                            Message = s.Message,
                            UserName = s.Email,
                            CareGiver = careGiver,
                            Children = new Collection<SignUpChild>() {
                                new SignUpChild() {
                                ChildUserName = c.ChildUserName,
                                ChildFirstName = c.ChildFirstName,
                                ChildLastName = c.ChildLastName,
                                ChildInfix = c.ChildInfix,
                                ChildBirthDate = c.ChildBirthDate,
                                Subject = c.Subject,
                                CareGiver = careGiver
                                }
                            }
                        });
                        await _context.SaveChangesAsync();
                        var sender = _email
                            .To(s.Email)
                            .Subject("Aanmelding ouder en kind succesvol aangevraagd")
                            .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is succesvol ontvangen, U krijgt zo snel mogelijk antwoord van de orthopedagoog.");
                        await sender.SendAsync();
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Success",
                            Message = "De aanmelding voor uw kind is successvol verzonden.",
                            Redirect = "Home",
                            Timeout = 2500
                        });
                    }
                    else
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Failed",
                            Message = "Uw kind is oud genoeg om een eigen account aan te maken.",
                            Redirect = "SignUp/Child",
                            Timeout = 2500
                        });
                }
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Er bestaat al een account met deze gebruikersnaam.",
                        Redirect = "SignUp/Child",
                        Timeout = 2500
                    });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Er bestaat al een account met dit e-mailadres.",
                    Redirect = "SignUp/Child",
                    Timeout = 2500
                });
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> AcceptSignUpAsync(SignUp s, string careGiver)
        {
            System.Console.WriteLine(s.TempId);
            var user = new AppUser
            {
                UserName = s.UserName,
                BirthDate = s.BirthDate,
                FirstName = s.FirstName,
                Infix = s.Infix,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Subject = s.Subject,
                CareGiver = careGiver
            };
            await _userManager.CreateAsync(user, "Test123!");
            await _userManager.AddToRoleAsync(user, "Tiener");
            // Api
            await _context.SaveChangesAsync();
            var sender = _email
                .To(s.Email)
                .Subject("Aanmelding goedgekeurd")
                .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is goedgekeurd, U kunt inloggen met dit wachtwoord: Test123!.");

            // ChatSystemController chatSystem = new ChatSystemController(_email, _userManager, _roleManager, _context);
            // var chat = chatSystem.CreatePrivateRoom(s.UserName);

            await DeleteSignUp(s.TempId);
            await sender.SendAsync();
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> AcceptSignUpWithChildren(SignUp s, SignUpChild c, string careGiver)
        {
            var child = new AppUser()
            {
                UserName = c.ChildUserName,
                FirstName = c.ChildFirstName,
                LastName = c.ChildLastName,
                Infix = c.ChildInfix,
                Email = "",
                Subject = c.Subject,
                BirthDate = c.ChildBirthDate,
                CareGiver = careGiver
            };
            await _context.SaveChangesAsync();
            await _userManager.CreateAsync(child, "Test123!");
            await _userManager.AddToRoleAsync(child, "Kind");
            var user = new AppUser
            {
                UserName = s.FirstName + s.LastName,
                Email = s.Email,
                Children = new Collection<AppUser>() { child },
                FirstName = s.FirstName,
                LastName = s.LastName,
                Infix = s.Infix,
                Subject = s.Subject,
                PhoneNumber = s.PhoneNumber,
                CareGiver = ""
            };
            await _context.SaveChangesAsync();
            await _userManager.CreateAsync(user, "Test123!");
            await _userManager.AddToRoleAsync(user, "Ouder");
            // Api
            await DeleteSignUp(s.TempId);
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
