using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserSystemController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserSystemController(UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager) 
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    private async Task<List<string>> GetRoles(IdentityUser user)
    {
        return new List<string>(await _userManager.GetRolesAsync(user));
    }
    [Authorize(Roles ="Moderator")]
    public async Task<IActionResult> Index()
    {
      var getUsers = await _userManager.Users.ToListAsync();
      var getRoleViewModel = new List<UserRoleViewModel>();  
      foreach(var user in getUsers)
      {
          var curentViewModel = new UserRoleViewModel()
          {
              UserId = user.Id,
              Email = user.Email,
              Roles = await GetRoles(user)
          };
          getRoleViewModel.Add(curentViewModel);
      }
      return View(getRoleViewModel);
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        var GetUser = _userManager.FindByIdAsync(id);
        await _userManager.DeleteAsync(GetUser.Result);
        return RedirectToAction("Index");
    }


}