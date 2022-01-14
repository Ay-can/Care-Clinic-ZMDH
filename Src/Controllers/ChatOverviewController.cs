using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ChatOverviewController : Controller
    {
        private readonly ILogger<ChatOverviewController> _logger;

        private readonly WdprContext _context;

        public ChatOverviewController(ILogger<ChatOverviewController> logger, WdprContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index() => View(await _context.Chats.ToListAsync());

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
