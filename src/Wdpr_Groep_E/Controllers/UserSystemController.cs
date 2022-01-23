using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    public class UserSystemController : Controller
    {
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserSystemController(WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<List<string>> GetRoles(AppUser user) => new List<string>(await _userManager.GetRolesAsync(user));

        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Index(string search, int page, int size, string sort)
        {
            var getRoleViewModel = new List<UserRoleViewModel>();

            foreach (var user in _context.Users)
            {
                getRoleViewModel.Add(new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    CaregiverUserName = _context.Users?.SingleOrDefault(s => s.Id == user.Caregiver)?.UserName,
                    Children = await _context.Users?.Where(u => u.Parent.Id == user.Id)?.ToListAsync(),
                    Roles = await GetRoles(user)
                });
            }

            if (sort == null) sort = "gebruiker_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < getRoleViewModel.Count;

            return View(Paginate(Search(Sort(getRoleViewModel.AsQueryable(), sort), search), page, size).ToList());
        }

        public IQueryable<UserRoleViewModel> Search(IQueryable<UserRoleViewModel> userRoleViews, string search)
        {
            if (search != null)
                userRoleViews = userRoleViews
                    .Where(s => s.UserName
                        .Contains(search) || s.Email
                                // .Contains(search) || s.Children.SingleOrDefault().FirstName
                                .Contains(search) || s.Roles.SingleOrDefault()
                                        // .Contains(search) || s.CaregiverUserName
                                        .Contains(search));
            return userRoleViews;
        }

        public IQueryable<UserRoleViewModel> Paginate(IQueryable<UserRoleViewModel> userRoleViews, int page, int size)
        {
            return userRoleViews.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<UserRoleViewModel> Sort(IQueryable<UserRoleViewModel> userRoleViews, string sort)
        {
            switch (sort)
            {
                case "gebruiker_oplopend":
                    return userRoleViews.OrderBy(r => r.UserName);
                case "gebruiker_aflopend":
                    return userRoleViews.OrderByDescending(r => r.UserName);
                case "email_oplopend":
                    return userRoleViews.OrderBy(r => r.Email);
                case "email_aflopend":
                    return userRoleViews.OrderByDescending(r => r.Email);
                // case "kind_oplopend":
                //     return userRoleViews.OrderBy(r => r.Children.SingleOrDefault().FirstName);
                // case "kind_aflopend":
                //     return userRoleViews.OrderByDescending(r => r.Children.SingleOrDefault().FirstName);
                case "rol_oplopend":
                    return userRoleViews.OrderBy(r => r.Roles.SingleOrDefault());
                case "rol_aflopend":
                    return userRoleViews.OrderByDescending(r => r.Roles.SingleOrDefault());
                case "ortho_oplopend":
                    return userRoleViews.OrderBy(r => r.CaregiverUserName);
                case "ortho_aflopend":
                    return userRoleViews.OrderByDescending(r => r.CaregiverUserName);
                default:
                    return userRoleViews.OrderBy(r => r);
            };
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Users.Include(u => u.Children).SingleOrDefault(u => u.Id == id).Children.Count == 0)
            {
                await _userManager.DeleteAsync(_userManager.FindByIdAsync(id).Result);
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Deze ouder heeft nog kinderen in het systeem staan, en kan niet worden verwijderd.",
                    Redirect = "UserSystem",
                    Timeout = 4000
                });
        }
    }
}
