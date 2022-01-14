using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;

        private readonly WdprContext _context;

        public ChatController(ILogger<ChatController> logger, WdprContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index() => View(await _context.Chats.ToListAsync());

        [HttpGet("{controller}/{id}")]
        public IActionResult Chat(int id)
        {
            var chat = _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == id);
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string userName, string text)
        {
            var message = new Message
            {
                ChatId = chatId,
                Text = text,
                Name = userName,
                Time = DateTime.Now
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = chatId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
