using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class ChatSystemController : Controller
    {
        private readonly IFluentEmail _email;
        private readonly WdprContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ChatSystemController(IFluentEmail email, WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _email = email;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string search, int page, int size, string sort)
        {
            var chats = Enumerable.Empty<Chat>().AsQueryable();
            if (User.IsInRole("Orthopedagoog"))
                chats = _context.Chats.Include(c => c.Users).Where(c => c.Type == ChatType.Room);
            else
                chats = _context.Chats
                    .Include(c => c.Users)
                        .Where(c => !c.Users
                            .Any(u => u.UserId == User
                                .FindFirst(ClaimTypes.NameIdentifier).Value) && c.Type == ChatType.Room);

            if (sort == null) sort = "chatnaam_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < chats.Count();

            return View(await Paginate(Search(Sort(chats, sort), search), page, size).ToListAsync());
        }

        public IQueryable<Chat> Search(IQueryable<Chat> chats, string search)
        {
            if (search != null)
                chats = chats.Where(c => c.Subject
                    .Contains(search) || c.AgeGroup
                        .Contains(search));
            return chats;
        }

        public IQueryable<Chat> Paginate(IQueryable<Chat> chats, int page, int size)
        {
            return chats.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<Chat> Sort(IQueryable<Chat> chats, string sort)
        {
            switch (sort)
            {
                case "chatnaam_oplopend":
                    return chats.OrderBy(c => c.Name);
                case "chatnaam_aflopend":
                    return chats.OrderByDescending(c => c.Name);
                case "onderwerp_oplopend":
                    return chats.OrderBy(c => c.Subject);
                case "onderwerp_aflopend":
                    return chats.OrderByDescending(c => c.Subject);
                case "deelnemers_oplopend":
                    return chats.OrderBy(c => c.Users.Count);
                case "deelnemers_aflopend":
                    return chats.OrderByDescending(c => c.Users.Count);
                case "leeftijd_oplopend":
                    return chats.OrderBy(c => c.AgeGroup);
                case "leeftijd_aflopend":
                    return chats.OrderByDescending(c => c.AgeGroup);
                default:
                    return chats.OrderBy(c => c.Name);
            }
        }

        public IActionResult Users(int id) => View(_context.ChatUsers
            .Include(cu => cu.User)
                .Include(cu => cu.Chat)
                    .ThenInclude(c => c.Messages)
                        .Where(u => u.ChatId == id)
                            .ToList());

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> BlockUser(string id, int chat)
        {
            _context.ChatUsers.SingleOrDefault(u => u.UserId == id && u.ChatId == chat).IsBlocked = true;
            var sender = _email
                .To(_context.Users.SingleOrDefault(u => u.Id == _context.Users.SingleOrDefault(u => u.Id == id).Caregiver).Email)
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
            _context.ChatUsers.SingleOrDefault(u => u.UserId == id && u.ChatId == chat).IsBlocked = false;
            _context.SaveChanges();
            return RedirectToAction("Users", new { id = chat });
        }

        [HttpPost]
        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreateRoom(string name, string age, string subject)
        {
            _context.Chats.Add(new Chat()
            {
                Id = GenerateChatId(),
                Name = name,
                Type = ChatType.Room,
                Subject = subject,
                AgeGroup = age,
                Users = { new ChatUser() { UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value } }
            });

            _context.Database.OpenConnection();
            try
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats ON;");
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats OFF;");
            }
            finally
            {
                _context.Database.CloseConnection();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Orthopedagoog")]
        public async Task<IActionResult> CreatePrivateRoom(string name)
        {
            var caregiver = _context.Users.Where(s => s.UserName == name).SingleOrDefault().Caregiver;
            var caregiverName = _context.Users.SingleOrDefault(s => s.Id == caregiver).UserName;

            var role = _context.Roles
                .SingleOrDefault(s => s.Id == _context.UserRoles
                    .SingleOrDefault(s => s.UserId == _context.Users
                        .SingleOrDefault(s => s.UserName == name).Id).RoleId).Name;

            var email = "";
            if (role == "Tiener") email = _context.Users.SingleOrDefault(s => s.UserName == name).Email;
            else if (role == "Kind") email = _context.Users.Include(p => p.Parent).SingleOrDefault(s => s.UserName == name).Parent.Email;

            var clientId = _userManager.Users.SingleOrDefault(u => u.UserName == name)?.Id;
            if (clientId != null)
            {
                var chat = new Chat
                {
                    Id = GenerateChatId(),
                    Name = "Chat tussen " + caregiverName + " en " + name,
                    Subject = _context.Users.SingleOrDefault(s => s.Id == caregiver).Subject,
                    Type = ChatType.Private,
                    Users = { new ChatUser() { UserId = _context.Users.SingleOrDefault(s => s.Id == caregiver).Id } }
                };
                _context.Chats.Add(chat);

                var sender = _email
                    .To(email)
                    .Subject("Chat aanvraag")
                    .Body($"{caregiverName} heeft een chat aangevraagd. Gebruik de volgende code om de chat te joinen: {chat.Id}");
                sender.Send();

                _context.Database.OpenConnection();
                try
                {
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats ON;");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats OFF;");
                }
                finally
                {
                    _context.Database.CloseConnection();
                }

                return RedirectToAction("Chat", "Chat", new { Id = chat.Id });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Deze gebruiker bestaat niet.",
                    Redirect = "ChatSystem",
                    Timeout = 2000
                });
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

        [Authorize(Roles = "Tiener, Kind")]
        public async Task<IActionResult> JoinPrivateRoom(int id)
        {
            var chat = _context.Chats.Include(c => c.Users).SingleOrDefault(c => c.Id == id);
            if (chat != null)
            {
                if (chat.Type == ChatType.Private)
                {
                    chat.Users.Add(new ChatUser
                    {
                        User = _userManager.Users.SingleOrDefault(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value),
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Chat", "Chat", new { id = chat.Id });
                }
                else
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Deze chat is niet privé.",
                        Redirect = "Chat",
                        Timeout = 2000
                    });
            }
            else
                return RedirectToAction("Index", "Message", new
                {
                    Type = "Failed",
                    Message = "Deze chat bestaat niet.",
                    Redirect = "Chat",
                    Timeout = 2000
                });
        }

        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            _context.Chats.Remove(await _context.Chats.FindAsync(id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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
    }
}
