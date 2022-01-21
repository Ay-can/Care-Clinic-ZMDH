using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private readonly WdprContext _context;

        public ChatController(IHubContext<ChatHub> chatHubContext, UserManager<AppUser> userManager, WdprContext context)
        {
            _chatContext = chatHubContext;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ChatUsers
                .Include(c => c.Chat)
                    .Where(c => c.UserId == HttpContext.User
                        .FindFirst(ClaimTypes.NameIdentifier).Value)
                            .ToListAsync());
        }

        [HttpGet("[controller]/{id}")]
        public IActionResult Chat(int id)
        {
            if (!_context.ChatUsers
                .Where(c => c.UserId == HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier).Value && c.ChatId == id)
                        .SingleOrDefault().IsBlocked)
            {
                return View(_context.Chats
                    .Include(c => c.Messages)
                        .SingleOrDefault(c => c.Id == id));
            }
            else
            {
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "U bent geblokkeerd in deze chat.",
                    Redirect = "Chat",
                    Timeout = 2000
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int id, string text)
        {
            _context.Messages.Add(new Message
            {
                ChatId = id,
                Text = text,
                Name = _userManager.GetUserName(User),
                Time = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = id });
        }

        [HttpPost("[controller]/[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomId)
        {
            await _chatContext.Groups.AddToGroupAsync(connectionId, roomId);
            return Ok();
        }

        [HttpPost("[controller]/[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomId)
        {
            await _chatContext.Groups.RemoveFromGroupAsync(connectionId, roomId);
            return Ok();
        }

        [HttpPost("[controller]/[action]")]
        public async Task<IActionResult> SendMessage(int id, string text)
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
            await _chatContext.Clients.Group(id.ToString()).SendAsync("ReceiveMessage", message);
            return Ok();
        }
    }
}
