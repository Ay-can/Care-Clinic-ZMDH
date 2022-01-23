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
            await roleManager.CreateAsync(new IdentityRole("Orthopedagoog"));
            await roleManager.CreateAsync(new IdentityRole("Moderator"));
            await roleManager.CreateAsync(new IdentityRole("Ouder"));
            await roleManager.CreateAsync(new IdentityRole("Tiener"));
            await roleManager.CreateAsync(new IdentityRole("Kind"));
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
            var Moderator = new AppUser
            {
                UserName = "Moderator",
                Email = "Moderator@email.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            await userManager.CreateAsync(Moderator, "Moderator123!");
            await userManager.AddToRoleAsync(Moderator, "Moderator");
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
                Subject = "ADD",
                WorkLocation = "ZMDH"
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
                Subject = "Faalangst",
                WorkLocation = "ZMDH"
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
                Subject = "ASS",
                WorkLocation = "ZMDH"
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
                Subject = "Hoogbegaafdheid",
                WorkLocation = "ZMDH"
            };
            await userManager.CreateAsync(Joseph, "Test123!");
            await userManager.AddToRoleAsync(Joseph, "Orthopedagoog");
        }
    }
}
