using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentEmail.Core;
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
        private readonly IFluentEmail _email;
        private readonly UserManager<AppUser> _userManager;
        // private readonly ILogger<ChatSystemController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly WdprContext _context;

        public ChatSystemController(IFluentEmail email, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, WdprContext context)
        {
            _email = email;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            if (User.IsInRole("Orthopedagoog"))
                return View(await Search(_context.Chats.Where(c => c.Type == ChatType.Room), search).ToListAsync());
            else
            {
                return View(await Search(_context.Chats.Include(c => c.Users)
                    .Where(c => !c.Users.Any(u => u.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value) && c.Type == ChatType.Room), search)
                    .ToListAsync());
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

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> BlockUser(string id, int chat)
        {
            _context.ChatUsers.FirstOrDefault(u => u.UserId == id && u.ChatId == chat).IsBlocked = true;
            var careGiver = _context.Users.SingleOrDefault(u => u.Id == id).CareGiver;
            var careGiverEmail = _context.Users.SingleOrDefault(u => u.Id == careGiver).Email;
            var sender = _email
                .To(careGiverEmail)
                .Subject("Cliënt geblokkeerd")
                .Body($"Uw cliënt: {_userManager.FindByIdAsync(id).Result.UserName} is geblokkeerd door een moderator in de chat: {_context.Chats.SingleOrDefault(c => c.Id == chat).Name}.");
            await _context.SaveChangesAsync();
            await sender.SendAsync();
            return RedirectToAction("Users", new { id = chat });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public IActionResult UnblockUser(string id, int chat)
        {
            _context.ChatUsers.FirstOrDefault(u => u.UserId == id && u.ChatId == chat).IsBlocked = false;
            _context.SaveChanges();
            return RedirectToAction("Users", new { id = chat });
        }

        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreatePrivateRoom(string name)
        {
            var getCareGiverKey = _context.Users.Where(s => s.UserName == name).SingleOrDefault().CareGiver;
            var getCareGiverName = _context.Users.SingleOrDefault(s => s.Id == getCareGiverKey).UserName;
            var getCareGiverId = _context.Users.SingleOrDefault(s => s.Id == getCareGiverKey).Id;
            var getCareGiverSubject = _context.Users.SingleOrDefault(s => s.Id == getCareGiverKey).Subject;

            var clientId = _userManager.Users.SingleOrDefault(u => u.UserName == name)?.Id;
            if (clientId != null)
            {
                var chat = new Chat
                {
                    Id = GenerateChatId(),
                    Name = "Chat tussen " + getCareGiverName + " en " + name,
                    Subject = getCareGiverSubject,
                    Type = ChatType.Private
                };
                var sender = _email
                    .To(_context.Users.SingleOrDefault(s => s.UserName == name).Email)
                    .Subject("Chat aanvraag")
                    .Body($"{getCareGiverName} heeft een chat aangevraagd. Gebruik de volgende code om de chat te joinen: {chat.Id}");
                sender.Send();
                chat.Users.Add(new ChatUser { UserId = getCareGiverId });
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
                    Timeout = 2500
                });
        }

        [Authorize(Roles = "Tiener, Kind")]
        public async Task<IActionResult> JoinPrivateRoom(int id)
        {
            var chat = _context.Chats.Include(c => c.Users).SingleOrDefault(c => c.Id == id);
            if (chat != null)
            {
                if (chat.Type == ChatType.Private)
                {
                    chat.Users.Add(new ChatUser { UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Chat", "Chat", new { id = chat.Id });
                }
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Deze chat is niet privé.",
                        Redirect = "Chat",
                        Timeout = 2500
                    });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Deze chat bestaat niet.",
                    Redirect = "Chat",
                    Timeout = 2500
                });
        }

        private int GenerateChatId()
        {
            var id = new Random().Next(100000, 999999);
            while (true)
            {
                if (_context.Chats.Any(c => c.Id == id))
                    id = new Random().Next(100000, 999999);
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

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
