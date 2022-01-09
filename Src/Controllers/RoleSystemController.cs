using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class RoleSystemController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSystemController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var GetRoles = await _roleManager.Roles.ToListAsync();
        return View(GetRoles);
    }
   //AddRoles....


   //DeleteRoles...




}