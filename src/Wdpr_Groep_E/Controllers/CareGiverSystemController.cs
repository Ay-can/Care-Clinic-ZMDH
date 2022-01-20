using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles ="Moderator")]
    public class CareGiverSystemController : Controller
    {
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public CareGiverSystemController(WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _usermanager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() 
        {
            List<AppUser> CareGiver = new List<AppUser>();
             var CareGiverRoleId = _context.Roles.SingleOrDefault(s => s.Name =="Orthopedagoog").Id;

             var GetCareGiverId = _context.UserRoles.Where(s => s.RoleId == CareGiverRoleId);

                foreach(var x in _context.Users)
                {
                    foreach(var z in GetCareGiverId.Where(s => s.UserId == x.Id))
                    {
                        CareGiver.Add(x);
                    }
                }    
            return View(CareGiver);
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
