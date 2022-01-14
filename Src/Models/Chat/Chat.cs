using System.Collections.Generic;

namespace Wdpr_Groep_E.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public string Subject { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<AppUser> Users { get; set; }
    }
}
