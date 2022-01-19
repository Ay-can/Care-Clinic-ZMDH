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
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<AppUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
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

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"Kan 2FA niet uitschakelen voor gebruiker met ID '{_userManager.GetUserId(User)}' omdat het momenteel niet is ingeschakeld.");
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

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Er is een onverwachte fout opgetreden bij het uitschakelen van 2FA voor gebruiker met ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("Gebruiker met ID '{UserId}' heeft 2fa uitgeschakeld.", _userManager.GetUserId(User));
            StatusMessage = "2fa is uitgeschakeld. U kunt 2fa inschakelen wanneer u een authenticatie-app instelt";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
