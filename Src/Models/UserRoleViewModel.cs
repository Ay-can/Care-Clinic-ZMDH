using System.Collections.Generic;

namespace Wdpr_Groep_E.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
