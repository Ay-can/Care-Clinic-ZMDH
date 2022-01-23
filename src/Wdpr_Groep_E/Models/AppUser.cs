using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Wdpr_Groep_E.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<ChatUser> Chats { get; set; }
        public ICollection<AppUser> Children { get; set; }

        public override string UserName { get; set; }

        public string FirstName { get; set; }
        public string Infix { get; set; }
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }
        public override string PhoneNumber { get; set; }

        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Addition { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public string Subject { get; set; }
        public string WorkLocation { get; set; }

        public string Caregiver { get; set; }
        public AppUser Parent { get; set; }
    }
}
