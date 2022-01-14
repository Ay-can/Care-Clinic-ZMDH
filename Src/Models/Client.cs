using System.ComponentModel.DataAnnotations;

namespace Wdpr_Groep_E.Models
{
//Dit moet in nl anders pakt de api het niet...
    public class Client
    {
        
        public string clientid{ get; set; }
        public string volledigenaam { get; set; }
        public string IBAN { get; set; }
        public string BSN { get; set; }
        public string gebdatum { get; set; }

    }
}