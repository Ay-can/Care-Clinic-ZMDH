using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account.Manage
{
    public partial class BSNModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IZmdhApi _api;

        public BSNModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender, IZmdhApi api)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _api = api;
        }

        public string Username { get; set; }
        [StringLength(9,MinimumLength = 9, ErrorMessage = "Vul een correct BSN in")]
        public string BSN { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string BSN { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
             var userId = user.Id;

            var getClientObject = await _api.GetClientObject(userId);
            BSN = getClientObject.BSN;

            Input = new InputModel
            {
                BSN = getClientObject.BSN,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeBSN()
        {
             
            var user = await _userManager.GetUserAsync(User);
            var userIdApi = user.Id;

            var getClientObject = await _api.GetClientObject(userIdApi);
            
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            await _api.PutClient(new Client() {clientid = int.Parse(userIdApi), BSN = Input.BSN});
            return RedirectToPage();
        }
     }
}
