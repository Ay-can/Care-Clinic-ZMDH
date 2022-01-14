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
        }

        public static async Task CreateModeratorAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //nog aanpassen wanneer default AppUser is veranderd.
            var Moderator = new AppUser
            {
                UserName = "Moderator@email.com",
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

        public static async Task CreateTestUserAsync(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            var TestUser = new AppUser
            {
                UserName = "test",
                Email = "test@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            }; 
           
                await userManager.CreateAsync(TestUser,"Test123!");
                await userManager.AddToRoleAsync(TestUser,"Test123!");
            
        }

    
    }
}
