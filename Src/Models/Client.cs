using System.ComponentModel.DataAnnotations;

namespace Wdpr_Groep_E.Models
{
    public class Client
    {
        [Key]
        public string ClientId { get; set; }
        public string FullName { get; set; }
        public string IBAN { get; set; }
        public string BSN { get; set; }
        public string BirthDate { get; set; }

    }
}