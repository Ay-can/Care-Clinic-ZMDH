using System.ComponentModel.DataAnnotations;

public class AppointmentModel
{
    public int Id { get; set; }

    [Required(ErrorMessage ="Vul uw Voornaam in.")]
    [Display(Name ="Voornaam")]
    public string FirstName { get; set; }
    [Display(Name ="Tussenvoegsel")]
    public string Infix { get; set; }
    [Required(ErrorMessage ="Vul uw Achternaam in.")]
    [Display(Name ="Achternaam")]
    public string LastName { get; set; }
    [EmailAddress( ErrorMessage = "Uw Email klopt niet.")]
    [Required(ErrorMessage = "Vul uw Email in.")]
    [Display(Name ="Email")]
    public string Email { get; set; }
    [Display(Name ="Telefoonnummer")]
    [DataType(DataType.PhoneNumber)]
    [StringLength(9)]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage ="Kies uw onderwerp.")]
    [Display(Name ="Onderwerp")]
    public string Subject { get; set; }
    [Required (ErrorMessage = "Voer uw reden voor een afspraak in")]
    [Display(Name ="Bericht")]
    [StringLength(250)]
    public string Message { get; set; }



}