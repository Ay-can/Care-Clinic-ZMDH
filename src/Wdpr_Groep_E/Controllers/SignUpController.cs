using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    public class SignUpController : Controller
    {
        private readonly IZmdhApi _api;
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SignUpController(IZmdhApi api, IFluentEmail email, WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _api = api;
            _email = email;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task<IActionResult> CreateSignUp(SignUp s, string caregiver)
        {
            if (!_userManager.Users.Any(u => u.Email == s.Email))
                if (!_userManager.Users.Any(u => u.UserName == s.UserName))
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
                            Caregiver = caregiver,
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
                            Timeout = 2000
                        });
                    }
                    else
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Failed",
                            Message = "U moet ouder dan 16 zijn om u aan te melden, laat anders je ouders/verzorgers je aanmelden.",
                            Redirect = "SignUp/Client",
                            Timeout = 3000
                        });
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Er bestaat al een account met deze gebruikersnaam.",
                        Redirect = "SignUp/Client",
                        Timeout = 2000
                    });
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Er bestaat al een account met dit e-mailadres.",
                    Redirect = "SignUp/Client",
                    Timeout = 2000
                });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignUpWithChild(SignUp s, SignUpChild c, string caregiver)
        {
            if (!_userManager.Users.Any(u => u.Email == s.Email))
                if (!_userManager.Users.Any(u => u.UserName == s.UserName))
                    if (c.ChildBirthDate > DateTime.Now.AddYears(-16))
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
                            Caregiver = caregiver,
                            Children = new Collection<SignUpChild>() {
                                new SignUpChild() {
                                ChildUserName = c.ChildUserName,
                                ChildFirstName = c.ChildFirstName,
                                ChildLastName = c.ChildLastName,
                                ChildInfix = c.ChildInfix,
                                ChildBirthDate = c.ChildBirthDate,
                                Subject = c.Subject,
                                Caregiver = caregiver,
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
                            Timeout = 2000
                        });
                    }
                    else
                        return RedirectToAction("Index", "Message", new
                        {
                            Type = "Failed",
                            Message = "Uw kind is oud genoeg om een eigen account aan te maken.",
                            Redirect = "SignUp/Child",
                            Timeout = 2000
                        });
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Er bestaat al een account met deze gebruikersnaam.",
                        Redirect = "SignUp/Child",
                        Timeout = 2000
                    });
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Er bestaat al een account met dit e-mailadres.",
                    Redirect = "SignUp/Child",
                    Timeout = 2000
                });
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> AcceptSignUpAsync(SignUp s, string caregiver)
        {
            int generateId = await _api.CreateClientId();
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
                Caregiver = caregiver,
                Id = generateId.ToString()
            };
            await _context.SaveChangesAsync();
            await _userManager.CreateAsync(user, "Test123!");
            await _userManager.AddToRoleAsync(user, "Tiener");

            var sender = _email
                .To(s.Email)
                .Subject("Aanmelding goedgekeurd")
                .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is goedgekeurd, U kunt inloggen met dit wachtwoord: Test123!.");
            await sender.SendAsync();

            await _api.PostClient(new Client()
            {
                clientid = generateId,
                volledigenaam = $"{user.FirstName} {user.Infix} {user.LastName}",
                IBAN = "",
                BSN = "",
                gebdatum = user.BirthDate.ToString()
            });

            await new ChatSystemController(_email, _context, _userManager, _roleManager).CreatePrivateRoom(s.UserName);
            await DeleteSignUp(s.TempId);
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> AcceptSignUpWithChildren(SignUp s, SignUpChild c, string caregiver)
        {
            int generateId = await _api.CreateClientId();
            var child = new AppUser()
            {
                UserName = c.ChildUserName,
                FirstName = c.ChildFirstName,
                LastName = c.ChildLastName,
                Infix = c.ChildInfix,
                Email = "",
                Subject = c.Subject,
                BirthDate = c.ChildBirthDate,
                Caregiver = caregiver,
                Id = generateId.ToString()
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
                Caregiver = "",
            };
            await _context.SaveChangesAsync();
            await _userManager.CreateAsync(user, "Test123!");
            await _userManager.AddToRoleAsync(user, "Ouder");

            child.Parent = user;

            var sender = _email
                .To(s.Email)
                .Subject("Aanmelding goedgekeurd")
                .Body($"Uw aanmelding voor een zmdh account over {s.Subject} is goedgekeurd, U kunt inloggen met dit wachtwoord: Test123!.");
            await sender.SendAsync();

            await _api.PostClient(new Client()
            {
                clientid = generateId,
                volledigenaam = $"{child.FirstName} {child.Infix} {child.LastName}",
                IBAN = "",
                BSN = "",
                gebdatum = child.BirthDate.ToString()
            });

            await new ChatSystemController(_email, _context, _userManager, _roleManager).CreatePrivateRoom(c.ChildUserName);
            await DeleteSignUp(s.TempId);
            return RedirectToAction("Overview", "SignUp");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSignUp(string Id)
        {
            _context.Remove(_context.SignUps.SingleOrDefault(s => s.TempId == Id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Overview");
        }
    }
}
