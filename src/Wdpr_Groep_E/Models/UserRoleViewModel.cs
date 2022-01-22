using System.Collections.Generic;

namespace Wdpr_Groep_E.Models
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            Children = new List<AppUser>();
            Roles = new List<string>();
        }

        public ICollection<AppUser> Children { get; set; }
        public ICollection<string> Roles { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Caregiver { get; set; }
        public string CaregiverUserName { get; set; }
    }
}
