using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles ="Moderator")]
    public class ClientSystemController : Controller
    {
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IZmdhApi _api;


        public ClientSystemController(WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IZmdhApi api)
        {
            _context = context;
            _usermanager = userManager;
            _roleManager = roleManager;
            _api = api;
        }

        public IActionResult Index() 
        {
            return View(_api.GetAllClients());
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
