using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ChatSystemController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ChatSystemController> _logger;
        private readonly WdprContext _context;

        public ChatSystemController(ILogger<ChatSystemController> logger, UserManager<AppUser> userManager, WdprContext context)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string search)
        {
            if (User.IsInRole("Orthopedagoog"))
                return View(await Search(_context.Chats, search).ToListAsync());
            else
            {
                return View(
                    await Search(
                        _context.Chats.Include(c => c.Users)
                        .Where(c => !c.Users.Any(
                            u => u.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                            ), search).ToListAsync());
            }
        }

        public IQueryable<Chat> Search(IQueryable<Chat> chats, string search)
        {
            if (search != null)
                chats = chats.Where(c => c.Subject.Contains(search) || c.AgeGroup.Contains(search));
            return chats;
        }

        public IActionResult Users(int id) => View(_context.ChatUsers.Include(c => c.User).Include(c => c.Chat).Where(u => u.ChatId == id).ToList());

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreateRoom(string name, string age, string subject)
        {
            var chat = new Chat()
            {
                Id = GenerateChatId(),
                Name = name,
                Type = ChatType.Room,
                Subject = subject,
                AgeGroup = age
            };
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Chat", new { id = chat.Id });
        }

        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreatePrivateRoom(string name)
        {
            var chat = new Chat
            {
                Id = GenerateChatId(),
                Name = "Chat tussen " + _userManager.GetUserAsync(User).Result.UserName + " en " + name,
                Subject = _userManager.GetUserAsync(User).Result.Subject,
                Type = ChatType.Private
            };
            var clientId = _userManager.Users.FirstOrDefault(u => u.UserName == name)?.Id;
            if (clientId != null)
            {
                chat.Users.Add(new ChatUser { UserId = clientId });
                chat.Users.Add(new ChatUser { UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value });
                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Chat", "Chat", new { Id = chat.Id });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Deze gebruiker bestaat niet.",
                    Redirect = "ChatSystem",
                    Timeout = 3000
                });
        }

        private int GenerateChatId()
        {
            var id = new Random().Next(10000, 99999);
            while (true)
            {
                if (_context.Chats.Any(c => c.Id == id))
                    id = new Random().Next(10000, 99999);
                else
                    break;
            }
            return id;
        }

        [HttpGet]
        [Authorize(Roles = "Tiener, Kind")]
        public async Task<IActionResult> JoinRoom(int id)
        {
            _context.ChatUsers.Add(new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", "Chat", new { Id = id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
