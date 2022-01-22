using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Voer uw {0} in.")]
            [Display(Name = "E-mail / Gebruikersnaam")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Voer uw {0} in.")]
            [DataType(DataType.Password, ErrorMessage = "{0} is verplicht.")]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; }

            [Display(Name = "Gegevens onthouden?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Identity/Account/Manage");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var GetUser = Input.Email;
                if (CheckEmail(Input.Email))
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        GetUser = user.UserName;
                    }
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(GetUser, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successvol ingelogd.");
                    // return LocalRedirect(returnUrl);
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Success",
                        Message = "Successvol ingelogd!",
                        Redirect = returnUrl,
                        Timeout = 2000
                    });
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account geblokkeerd door te veel pogingen.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inloggen mislukt.");
                    return RedirectToAction("Index", "Message", new
                    {
                        Type = "Failed",
                        Message = "Verkeerd ingevoerde gegevens, probeer opnieuw!",
                        Redirect = "Identity/Account/Login",
                        Timeout = 2000
                    });
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public bool CheckEmail(string email)
        {
            try
            {
                MailAddress getEmail = new MailAddress(email);
                return true;
            }
            catch
            {
                Console.WriteLine("Geen correcte email!");
                return false;
            }
        }
    }
}
