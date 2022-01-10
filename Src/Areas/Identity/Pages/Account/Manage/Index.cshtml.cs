using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} is verplicht.")]
            [StringLength(100, ErrorMessage = "Uw {0} mag mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Voornaam")]
            public string FirstName { get; set; }

            [StringLength(50, ErrorMessage = "Uw {0} mag mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Tussenvoegsel")]
            public string Infix { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [StringLength(100, ErrorMessage = "Uw {0} mag mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Achternaam")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [DataType(DataType.Date)]
            [Display(Name = "Geboortedatum")]
            public DateTime BirthDate
            {
                get { return this.dateCreated.HasValue ? this.dateCreated.Value : DateTime.Now; }
                set { this.dateCreated = value; }
            }
            private DateTime? dateCreated = null;

            [Phone]
            [Display(Name = "Telefoon nummer")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Straat")]
            public string Street { get; set; }

            [Required]
            [Display(Name = "Huisnummer")]
            public string HouseNumber { get; set; }

            [Required]
            [Display(Name = "Toevoeging")]
            public string Addition { get; set; }

            [Required]
            [Display(Name = "Postcode")]
            public string ZipCode { get; set; }

            [Required]
            [Display(Name = "Woonplaats")]
            public string City { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
