using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class RoleSystemController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSystemController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

        [HttpGet]
        public async Task<IActionResult> Index(string search, int page, int size, string sort)
        {
            var roles = _roleManager.Roles;
            if (sort == null) sort = "rol_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < roles.Count();

            return View(await Paginate(Search(Sort(roles, sort), search), page, size).ToListAsync());
        }

        public IQueryable<IdentityRole> Search(IQueryable<IdentityRole> reports, string search)
        {
            if (search != null) reports = reports.Where(r => r.Name.Contains(search));
            return reports;
        }

        public IQueryable<IdentityRole> Paginate(IQueryable<IdentityRole> reports, int page, int size)
        {
            return reports.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<IdentityRole> Sort(IQueryable<IdentityRole> reports, string sort)
        {
            switch (sort)
            {
                case "rol_oplopend":
                    return reports.OrderBy(r => r.Name);
                case "rol_aflopend":
                    return reports.OrderByDescending(r => r.Name);
                default:
                    return reports.OrderBy(r => r);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string name)
        {
            if (name != null) await _roleManager.CreateAsync(new IdentityRole(name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            await _roleManager.DeleteAsync(await _roleManager.FindByIdAsync(id));
            return RedirectToAction("Index");
        }
    }
}
