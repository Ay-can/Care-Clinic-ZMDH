using System.ComponentModel.DataAnnotations;

namespace Wdpr_Groep_E.Models
{
    public class Caregiver
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Infix { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}
