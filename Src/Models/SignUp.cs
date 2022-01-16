using System.ComponentModel.DataAnnotations;

namespace Wdpr_Groep_E.Models
{
    public class SignUp
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vul uw {0} in.")]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        [Display(Name = "Tussenvoegsel")]
        public string Infix { get; set; }

        [Required(ErrorMessage = "Vul uw {0} in.")]
        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Uw {0} klopt niet.")]
        [Required(ErrorMessage = "Vul uw {0} in.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Uw {0} klopt niet.")]
        [Display(Name = "Telefoonnummer")]
        [StringLength(10, ErrorMessage = "Een {0} kan maximaal {1} karakters lang zijn.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Kies uw onderwerp.")]
        [Display(Name = "Onderwerp")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Voer uw reden voor een afspraak in")]
        [Display(Name = "Bericht")]
        [StringLength(250)]
        public string Message { get; set; }
    }
}
