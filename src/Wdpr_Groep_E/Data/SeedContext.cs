using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Data
{
    public static class SeedContext
    {
        public static async Task CreateRolesAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //We maken rollen aan en vervolgens voegen we ze toe.
            await roleManager.CreateAsync(new IdentityRole("Orthopedagoog"));
            await roleManager.CreateAsync(new IdentityRole("Moderator"));
            await roleManager.CreateAsync(new IdentityRole("Ouder"));
            await roleManager.CreateAsync(new IdentityRole("Tiener"));
            await roleManager.CreateAsync(new IdentityRole("Kind"));

            await roleManager.CreateAsync(new IdentityRole("TestUser"));
        }

        public static async Task CreateSubjects(WdprContext context)
        {
            if (context.Subjects.Any()) return;
            await context.Subjects.AddAsync(new Subject { Name = "ADD" });
            await context.Subjects.AddAsync(new Subject { Name = "ASS" });
            await context.Subjects.AddAsync(new Subject { Name = "Faalangst" });
            await context.Subjects.AddAsync(new Subject { Name = "Hoogbegaafdheid" });
            await context.SaveChangesAsync();
        }

        public static async Task CreateModeratorAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //nog aanpassen wanneer default AppUser is veranderd.
            var Moderator = new AppUser
            {
                UserName = "Moderator",
                Email = "Moderator@email.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            if (userManager.Users.All(s => s.Id != Moderator.Id))
            {
                var User = await userManager.FindByEmailAsync(Moderator.Email);
                if (User == null)
                {
                    await userManager.CreateAsync(Moderator, "Moderator123!");
                    await userManager.AddToRoleAsync(Moderator, "Moderator");
                }
            }
        }

        public static async Task CreateOrthopedagogen(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var Angela = new AppUser
            {
                UserName = "AngelaVanHeringa",
                FirstName = "Angela",
                Infix = "van",
                LastName = "Heringa",
                Email = "angela@zmdh.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "ADD"
            };
            await userManager.CreateAsync(Angela, "Test123!");
            await userManager.AddToRoleAsync(Angela, "Orthopedagoog");

            var Gijs = new AppUser
            {
                UserName = "GijsBroekman",
                FirstName = "Gijs",
                LastName = "Broekman",
                Email = "gijs@zmdh.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "Faalangst"
            };
            await userManager.CreateAsync(Gijs, "Test123!");
            await userManager.AddToRoleAsync(Gijs, "Orthopedagoog");

            var Jantinus = new AppUser
            {
                UserName = "JantinusVerduin",
                FirstName = "Jantinus",
                LastName = "Verduin",
                Email = "jantinus@zmdh.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "ASS"
            };
            await userManager.CreateAsync(Jantinus, "Test123!");
            await userManager.AddToRoleAsync(Jantinus, "Orthopedagoog");

            var Joseph = new AppUser
            {
                UserName = "JosephVanDerVliet",
                FirstName = "Joseph",
                Infix = "van der",
                LastName = "Vliet",
                Email = "joseph@zmdh.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "Hoogbegaafdheid"
            };
            await userManager.CreateAsync(Joseph, "Test123!");
            await userManager.AddToRoleAsync(Joseph, "Orthopedagoog");
        }

        public static async Task CreateUsers(WdprContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var Test = new AppUser
            {
                UserName = "Test",
                Email = "zmdh.hulp@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "Faalangst",
                CareGiver = context.Users.Where(u => u.Subject == "Faalangst").FirstOrDefault().Id
            };
            await userManager.CreateAsync(Test, "Test123!");
            await userManager.AddToRoleAsync(Test, "Tiener");

            var Hans = new AppUser
            {
                UserName = "Hans2207",
                Email = "hans@mail.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "Hoogbegaafdheid",
                CareGiver = context.Users.Where(u => u.Subject == "Hoogbegaafdheid").FirstOrDefault().Id
            };
            await userManager.CreateAsync(Hans, "Test123!");
            await userManager.AddToRoleAsync(Hans, "Tiener");

            var Sara = new AppUser
            {
                UserName = "Sara2004",
                Email = "sara@mail.nl",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Subject = "ADD",
                CareGiver = context.Users.Where(u => u.Subject == "ADD").FirstOrDefault().Id
            };
            await userManager.CreateAsync(Sara, "Test123!");
            await userManager.AddToRoleAsync(Sara, "Kind");
        }
    }
}
