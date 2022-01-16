using System.Collections.Generic;

namespace Wdpr_Groep_E.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
        }

        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public string Subject { get; set; }
        public string AgeGroup { get; set; }
    }
}
