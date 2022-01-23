using System.Linq;
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
    [Authorize(Roles = "Moderator, Orthopedagoog")]
    public class ClientController : Controller
    {
        private readonly IZmdhApi _api;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClientController(IZmdhApi api, WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _api = api;
            _context = context;
            _usermanager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string search, int page, int size, string sort)
        {
            var clients = _api.GetAllClients().Result.AsQueryable();

            if (sort == null) sort = "id_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < clients.Count();

            ViewData["total"] = clients.Count() / size;

            return View(Paginate(Search(Sort(clients, sort), search), page, size).ToList());
        }

        public IQueryable<string> Search(IQueryable<string> clients, string search)
        {
            if (search != null) clients = clients.Where(c => c.Contains(search));
            return clients;
        }

        public IQueryable<string> Paginate(IQueryable<string> clients, int page, int size)
        {
            return clients.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<string> Sort(IQueryable<string> clients, string sort)
        {
            switch (sort)
            {
                case "id_oplopend":
                    return clients.OrderBy(c => c);
                case "id_aflopend":
                    return clients.OrderByDescending(c => c);
                default:
                    return clients.OrderBy(c => c);
            }
        }

        [HttpPost]
        public IActionResult ClientInfo(string clientId) => View(_api.GetClientObject(clientId).Result);
    }
}
