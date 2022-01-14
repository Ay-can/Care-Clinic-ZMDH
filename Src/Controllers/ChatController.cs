using System.Diagnostics;
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

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> CreateChat(string name)
        {
            _context.Chats.Add(new Chat()
            {
                Name = name,
                Type = ChatType.Chat
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupChat(string name, string subject)
        {
            _context.Chats.Add(new Chat()
            {
                Name = name,
                Type = ChatType.GroupChat,
                Subject = subject
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
