using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ReportSystemController : Controller
    {
        private readonly IFluentEmail _email;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ReportSystemController> _logger;
        private readonly WdprContext _context;

        public ReportSystemController(ILogger<ReportSystemController> logger, UserManager<AppUser> userManager, IFluentEmail email, WdprContext context)
        {
            _email = email;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Moderator")]
        public IActionResult Index() => View(_context.Reports);

        [HttpPost]
        public async Task<IActionResult> ReportUser(int chat, string name, string text)
        {
            _context.Reports.Add(new Report
            {
                Name = name,
                Text = text,
                Date = DateTime.Today,
                ChatId = chat
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Chat", new { id = chat });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> BlockReportedUser(int report, string id, int chat)
        {
            _context.ChatUsers.SingleOrDefault(u => u.UserId == id && u.ChatId == chat).IsBlocked = true;
            var careGiver = _context.Users.SingleOrDefault(u => u.Id == id).CareGiver;
            var careGiverEmail = _context.Users.SingleOrDefault(u => u.Id == careGiver).Email;
            var sender = _email
                .To(careGiverEmail)
                .Subject("Cliënt geblokkeerd")
                .Body($"Uw cliënt: {_userManager.FindByIdAsync(id).Result.UserName} is geblokkeerd door een moderator in de chat: {_context.Chats.SingleOrDefault(c => c.Id == chat).Name}.");
            await RemoveReport(report);
            await _context.SaveChangesAsync();
            await sender.SendAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
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
