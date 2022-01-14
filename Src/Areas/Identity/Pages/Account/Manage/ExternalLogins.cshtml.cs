using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalLoginsModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID 'user.Id' niet laden.");
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID 'user.Id' niet laden.");
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = "De externe login is niet verwijderd.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "De externe login is verwijderd.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID 'user.Id' niet laden.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new InvalidOperationException($"Er is een onverwachte fout opgetreden bij het laden van externe inloggegevens voor gebruiker met ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = "De externe login is niet toegevoegd. Externe logins kunnen slechts aan één account worden gekoppeld.";
                return RedirectToPage();
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "De externe login is toegevoegd.";
            return RedirectToPage();
        }
    }
}
