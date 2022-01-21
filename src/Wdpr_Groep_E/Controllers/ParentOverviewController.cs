using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles = "Ouder")]
    public class ParentOverviewController : Controller
    {
        private readonly WdprContext _context;

        public ParentOverviewController(WdprContext context) => _context = context;

        public IActionResult Index() => View(_context.Chats
            .Include(c => c.Messages)
                .Include(c => c.Users)
                    .ThenInclude(cu => cu.User)
                        .ThenInclude(u => u.Parent)
                            .Where(c => c.Type == ChatType.Private));
    }
}
