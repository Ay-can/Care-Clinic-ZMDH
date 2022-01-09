using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public static class SeedContext
{
    public static async Task CreateRolesAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //We maken rollen aan en vervolgens voegen we ze toe.
        await roleManager.CreateAsync(new IdentityRole("Moderator"));
        await roleManager.CreateAsync(new IdentityRole("Ouder"));
        await roleManager.CreateAsync(new IdentityRole("Tiener"));
        await roleManager.CreateAsync(new IdentityRole("Kind"));
    }

    public static async Task CreateModeratorAsync(UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager)
    {
        //nog aanpassen wanneer default IdentityUser is veranderd.
        var Moderator = new IdentityUser
        {
            UserName = "Moderator@email.com",
            Email = "Moderator@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };
        if(userManager.Users.All(s => s.Id != Moderator.Id))
        {
            var User = await userManager.FindByEmailAsync(Moderator.Email);
            if(User == null)
            {
                await userManager.CreateAsync(Moderator,"Moderator123!");
                await userManager.AddToRoleAsync(Moderator,"Moderator");
            }
        }
    }
}