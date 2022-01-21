using System.Collections.Generic;
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
        public async Task<IActionResult> Index()
        {
            var getRoleViewModel = new List<UserRoleViewModel>();
            foreach (var user in await _userManager.Users.ToListAsync())
            {
                var currentViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Children = await _context.Users.Where(u => u.Parent.Id == user.Id).ToListAsync(),
                    Roles = await GetRoles(user)
                };
                getRoleViewModel.Add(currentViewModel);
            }
            return View(getRoleViewModel);
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
