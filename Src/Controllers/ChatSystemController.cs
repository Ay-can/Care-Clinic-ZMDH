using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ChatSystemController : Controller
    {
        private readonly ILogger<ChatSystemController> _logger;

        private readonly WdprContext _context;

        public ChatSystemController(ILogger<ChatSystemController> logger, WdprContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var chats = _context.Chats
                .Include(c => c.Users)
                .Where(c => !c.Users.Any(u => u.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            return View(chats);
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreateGroupChat(string name, string subject)
        {
            var chat = new Chat()
            {
                Name = name,
                Type = ChatType.GroupChat,
                Subject = subject
            };

            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinChat(int id)
        {
            _context.ChatUsers.Add(new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Chat", new { id = id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
