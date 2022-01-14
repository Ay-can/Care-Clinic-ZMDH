using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class CaregiverController : Controller
    {
        private readonly ILogger<CaregiverController> _logger;

        private readonly WdprContext _context;

        public CaregiverController(ILogger<CaregiverController> logger, WdprContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index() => View(await _context.Caregivers.ToListAsync());

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
