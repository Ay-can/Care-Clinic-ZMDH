using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Moderator")]
public class RoleSystemController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSystemController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var GetRoles = await _roleManager.Roles.ToListAsync();
        return View(GetRoles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string Name)
    {
        if (Name != null)
            await _roleManager.CreateAsync(new IdentityRole(Name));
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var GetRole = await _roleManager.FindByIdAsync(id);
        if (GetRole == null)
        {
            // Return not found view of andere pagina.
        }
        await _roleManager.DeleteAsync(GetRole);
        return RedirectToAction("Index");
    }
}
