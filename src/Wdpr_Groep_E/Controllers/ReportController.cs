using System;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ReportController : Controller
    {
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReportController(IFluentEmail email, WdprContext context, UserManager<AppUser> userManager)
        {
            _email = email;
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

            var caregiverEmail = _context.Users
                .SingleOrDefault(u => u.Id == _context.Users
                    .SingleOrDefault(u => u.Id == id).Caregiver).Email;

            var sender = _email
                .To(caregiverEmail)
                .Subject("Cliënt geblokkeerd")
                .Body($"Uw cliënt: {_userManager.FindByIdAsync(id).Result.UserName} is geblokkeerd door een moderator in de chat: {_context.Chats.SingleOrDefault(c => c.Id == chat).Name}.");
            await sender.SendAsync();

            await RemoveReport(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> RemoveReport(int id)
        {
            _context.Reports.Remove(await _context.Reports.FindAsync(id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
