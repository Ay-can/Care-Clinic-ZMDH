using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
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
            [StringLength(100, ErrorMessage = "Uw {0} mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Voornaam")]
            public string FirstName { get; set; }

            [StringLength(50, ErrorMessage = "Uw {0} mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Tussenvoegsel")]
            public string Infix { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [StringLength(100, ErrorMessage = "Uw {0} mag maximaal {1} karakters lang zijn.")]
            [Display(Name = "Achternaam")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [DataType(DataType.Date)]
            [Display(Name = "Geboortedatum")]
            public DateTime BirthDate { get; set; }

            [Phone(ErrorMessage = "Uw {0} is niet juist.")]
            [Display(Name = "Telefoonnummer")]
            [StringLength(10, ErrorMessage = "Een {0} kan maximaal {1} karakters lang zijn.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [Display(Name = "Straat")]
            public string Street { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [Display(Name = "Huisnummer")]
            public string HouseNumber { get; set; }

            [Display(Name = "Toevoeging")]
            public string Addition { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [Display(Name = "Postcode")]
            public string ZipCode { get; set; }

            [Required(ErrorMessage = "{0} is verplicht.")]
            [Display(Name = "Woonplaats")]
            public string City { get; set; }
            [Display(Name = "Onderwerp")]
            public string Subject { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var birthDate = user.BirthDate;
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var infix = user.Infix;
            var street = user.Street;
            var houseNumber = user.HouseNumber;
            var addition = user.Addition;
            var zipCode = user.ZipCode;
            var city = user.City;
            var subject = user.Subject;
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                BirthDate = birthDate,
                FirstName = firstName,
                LastName = lastName,
                Infix = infix,
                Street = street,
                HouseNumber = houseNumber,
                Addition = addition,
                ZipCode = zipCode,
                City = city,
                Subject = subject
                
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Niet instaat om user op te halen met ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Niet instaat om user op te halen met ID '{_userManager.GetUserId(User)}'.");
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
                    StatusMessage = "Een onverwachte error heeft zich voorgedaan.";
                    return RedirectToPage();
                }
            }

            var birthDate = user.BirthDate;
            if (Input.BirthDate != birthDate)
            {
                user.BirthDate = Input.BirthDate;
                await _userManager.UpdateAsync(user);
            }

            var firstname = user.FirstName;
            if (Input.FirstName != firstname)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }

            var lastname = user.LastName;
            if (Input.LastName != lastname)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
            }

            var infix = user.Infix;
            if (Input.Infix != infix)
            {
                user.Infix = Input.Infix;
                await _userManager.UpdateAsync(user);
            }

            var street = user.Street;
            if (Input.Street != street)
            {
                user.Street = Input.Street;
                await _userManager.UpdateAsync(user);
            }

            var housenumber = user.HouseNumber;
            if (Input.HouseNumber != housenumber)
            {
                user.HouseNumber = Input.HouseNumber;
                await _userManager.UpdateAsync(user);
            }

            var addition = user.Addition;
            if (Input.Addition != addition)
            {
                user.Addition = Input.Addition;
                await _userManager.UpdateAsync(user);
            }

            var zipcode = user.ZipCode;
            if (Input.ZipCode != zipcode)
            {
                user.ZipCode = Input.ZipCode;
                await _userManager.UpdateAsync(user);
            }

            var city = user.City;
            if (Input.City != city)
            {
                user.City = Input.City;
                await _userManager.UpdateAsync(user);
            }
              var subject = user.Subject;
            if (Input.Subject != subject)
            {
                user.Subject = Input.Subject;
                await _userManager.UpdateAsync(user);
            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Uw profiel is geupdate.";
            return RedirectToPage();
        }
    }
}
