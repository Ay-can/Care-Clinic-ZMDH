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
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _chatContext;

        public ChatController(WdprContext context, UserManager<AppUser> userManager, IHubContext<ChatHub> chatHubContext)
        {
            _context = context;
            _userManager = userManager;
            _chatContext = chatHubContext;
        }

        public async Task<IActionResult> Index(string search, int page, int size, string sort)
        {
            var chatUsers = _context.ChatUsers
                .Include(c => c.Chat)
                    .Where(c => c.UserId == HttpContext.User
                        .FindFirst(ClaimTypes.NameIdentifier).Value);

            if (sort == null) sort = "chatnaam_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < chatUsers.Count();

            return View(await Paginate(Search(Sort(chatUsers, sort), search), page, size).ToListAsync());
        }

        public IQueryable<ChatUser> Search(IQueryable<ChatUser> chatUsers, string search)
        {
            if (search != null) chatUsers = chatUsers.Where(u => u.Chat.Name.Contains(search));
            return chatUsers;
        }

        public IQueryable<ChatUser> Paginate(IQueryable<ChatUser> chatUsers, int page, int size)
        {
            return chatUsers.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<ChatUser> Sort(IQueryable<ChatUser> chatUsers, string sort)
        {
            switch (sort)
            {
                case "chatnaam_oplopend":
                    return chatUsers.OrderBy(cu => cu.Chat.Name);
                case "chatnaam_aflopend":
                    return chatUsers.OrderByDescending(cu => cu.Chat.Name);
                case "onderwerp_oplopend":
                    return chatUsers.OrderBy(cu => cu.Chat.Subject);
                case "onderwerp_aflopend":
                    return chatUsers.OrderByDescending(cu => cu.Chat.Subject);
                case "type_oplopend":
                    return chatUsers.OrderBy(cu => cu.Chat.Type);
                case "type_aflopend":
                    return chatUsers.OrderByDescending(cu => cu.Chat.Type);
                default:
                    return chatUsers.OrderBy(cu => cu.Chat.Name);
            }
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
