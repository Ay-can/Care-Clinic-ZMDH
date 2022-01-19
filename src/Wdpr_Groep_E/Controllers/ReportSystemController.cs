using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ReportSystemController : Controller
    {
        private readonly ILogger<ReportSystemController> _logger;
        private readonly WdprContext _context;

        public ReportSystemController(ILogger<ReportSystemController> logger, WdprContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View(_context.Reports);

        [HttpPost]
        public async Task<IActionResult> ReportUser(int chat, string name, string text)
        {
            _context.Reports.Add(new Report
            {
                Name = name,
                Text = text,
                Date = DateTime.Today,
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Chat", new { id = chat });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
