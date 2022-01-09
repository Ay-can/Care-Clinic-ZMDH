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
}