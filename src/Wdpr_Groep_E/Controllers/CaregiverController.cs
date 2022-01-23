using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class CaregiverController : Controller
    {
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CaregiverController(WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _usermanager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string search, int page, int size, string sort)
        {
            var caregivers = _context.Users
                .SelectMany(x => _context.UserRoles
                    .Where(s => s.RoleId == _context.Roles
                        .SingleOrDefault(s => s.Name == "Orthopedagoog").Id)
                            .Where(s => s.UserId == x.Id)
                                .Select(z => x));

            if (sort == null) sort = "voornaam_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < caregivers.Count();

            return View(await Paginate(Search(Sort(caregivers, sort), search), page, size).ToListAsync());
        }

        public IQueryable<AppUser> Search(IQueryable<AppUser> users, string search)
        {
            if (search != null)
                users = users.Where(u => u.FirstName
                    .Contains(search) || u.LastName
                        .Contains(search) || u.Subject
                            .StartsWith(search));
            return users;
        }

        public IQueryable<AppUser> Paginate(IQueryable<AppUser> users, int page, int size)
        {
            return users.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<AppUser> Sort(IQueryable<AppUser> users, string sort)
        {
            switch (sort)
            {
                case "voornaam_oplopend":
                    return users.OrderBy(h => h.FirstName);
                case "voornaam_aflopend":
                    return users.OrderByDescending(h => h.FirstName);
                case "achternaam_oplopend":
                    return users.OrderBy(h => h.LastName);
                case "achternaam_aflopend":
                    return users.OrderByDescending(h => h.LastName);
                case "onderwerp_oplopend":
                    return users.OrderBy(h => h.Subject);
                case "onderwerp_aflopend":
                    return users.OrderByDescending(h => h.Subject);
                default:
                    return users.OrderBy(h => h.FirstName);
            }
        }

        public IActionResult Users(string id) => View(_context.Users
            .Where(u => u.Caregiver == _usermanager
                .FindByIdAsync(id).Result.Id));
    }
}
