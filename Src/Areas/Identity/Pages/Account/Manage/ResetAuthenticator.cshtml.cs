using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        ILogger<ResetAuthenticatorModel> _logger;

        public ResetAuthenticatorModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("Gebruiker met ID '{UserId}' heeft hun authenticatie-app-sleutel opnieuw ingesteld.", user.Id);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Uw authenticator-app-sleutel is opnieuw ingesteld, u moet uw authenticator-app configureren met de nieuwe sleutel.";

            return RedirectToPage("./EnableAuthenticator");
        }
    }
}
