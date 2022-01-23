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
        public async Task<IActionResult> Overview(string search, int page, int size, string sort)
        {
            var signups = _context.SignUps.Include(s => s.Children);
            if (sort == null) sort = "voornaam_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < signups.Count();

            return View(await Paginate(Search(Sort(signups, sort), search), page, size).ToListAsync());
        }

        public IQueryable<SignUp> Search(IQueryable<SignUp> signups, string search)
        {
            if (search != null) signups = signups
                .Where(s => s.FirstName
                    .Contains(search) || s.LastName
                        .Contains(search) || s.Email
                            .Contains(search) || s.PhoneNumber
                                .Contains(search) || s.Subject
                                    .Contains(search) || s.Children.SingleOrDefault().ChildFirstName
                                        .Contains(search) || s.Children.SingleOrDefault().ChildLastName
                                            .Contains(search) || s.Children.SingleOrDefault().Caregiver
                                                .Contains(search));
            return signups;
        }

        public IQueryable<SignUp> Paginate(IQueryable<SignUp> signups, int page, int size)
        {
            return signups.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<SignUp> Sort(IQueryable<SignUp> signups, string sort)
        {
            switch (sort)
            {
                case "voornaam_oplopend":
                    return signups.OrderBy(r => r.FirstName);
                case "voornaam_aflopend":
                    return signups.OrderByDescending(r => r.FirstName);
                case "achternaam_oplopend":
                    return signups.OrderBy(r => r.LastName);
                case "achternaam_aflopend":
                    return signups.OrderByDescending(r => r.LastName);
                case "email_oplopend":
                    return signups.OrderBy(r => r.Email);
                case "email_aflopend":
                    return signups.OrderByDescending(r => r.Email);
                case "telefoon_oplopend":
                    return signups.OrderBy(r => r.PhoneNumber);
                case "telefoon_aflopend":
                    return signups.OrderByDescending(r => r.PhoneNumber);
                case "onderwerp_oplopend":
                    return signups.OrderBy(r => r.Subject);
                case "onderwerp_aflopend":
                    return signups.OrderByDescending(r => r.Subject);
                case "kind_oplopend":
                    return signups.OrderBy(r => r.Children.SingleOrDefault().ChildFirstName);
                case "kind_aflopend":
                    return signups.OrderByDescending(r => r.Children.SingleOrDefault().ChildFirstName);
                default:
                    return signups.OrderBy(r => r);
            }
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
                            Caregiver = caregiver
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
                                    Caregiver = caregiver
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
        public async Task<IActionResult> AcceptSignUp(SignUp s, string caregiver)
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
        public async Task<IActionResult> AcceptSignUpWithChild(SignUp s, SignUpChild c, string caregiver)
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
                Caregiver = ""
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
