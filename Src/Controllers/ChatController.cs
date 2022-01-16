using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Hubs;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles = "Tiener, Kind, Orthopedagoog")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chatContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ChatController> _logger;
        private readonly WdprContext _context;

        public ChatController(
            IHubContext<ChatHub> chatHubContext,
            UserManager<AppUser> userManager,
            ILogger<ChatController> logger,
            WdprContext context)
        {
            _chatContext = chatHubContext;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var chats = await _context.ChatUsers
                .Include(c => c.Chat)
                .Where(c => c.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToListAsync();
            return View(chats);
        }

        [HttpGet("[controller]/{id}")]
        public IActionResult Chat(int id)
        {
            var chat = _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == id);
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int id, string text)
        {
            var message = new Message
            {
                ChatId = id,
                Text = text,
                Name = _userManager.GetUserName(User),
                Time = DateTime.Now
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = id });
        }

        // SignalR methodes

        [HttpPost("[controller]/[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomName)
        {
            await _chatContext.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[controller]/[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _chatContext.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[controller]/[action]")]
        public async Task<IActionResult> SendMessage(int id, string room, string text)
        {
            var message = new Message
            {
                ChatId = id,
                Text = text,
                Name = _userManager.GetUserName(User),
                Time = DateTime.Now
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await _chatContext.Clients.Group(room)
                .SendAsync("ReceiveMessage", message);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
