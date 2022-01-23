using System;
using System.ComponentModel.DataAnnotations;

namespace Wdpr_Groep_E.Models
{
    public class SignUpChild
    {
        public SignUp Signup { get; set; }

        public string TempChildId { get; set; } = Guid.NewGuid().ToString();

        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is verplicht")]
        [Display(Name = "Gebruikersnaam")]
        public string ChildUserName { get; set; }

        [Required(ErrorMessage = "Vul uw {0} in.")]
        [Display(Name = "Voornaam")]
        public string ChildFirstName { get; set; }

        [Display(Name = "Tussenvoegsel")]
        public string ChildInfix { get; set; }

        [Required(ErrorMessage = "Vul uw {0} in.")]
        [Display(Name = "Achternaam")]
        public string ChildLastName { get; set; }

        [Required(ErrorMessage = "Kies een {0}.")]
        [Display(Name = "Onderwerp")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "U moet ouder dan 16 zijn.")]
        [Display(Name = "Leeftijd")]
        public DateTime ChildBirthDate { get; set; }

        [Required]
        [Display(Name = "Orthopedagoog")]
        public string Caregiver { get; set; }
    }
}
