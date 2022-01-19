namespace Wdpr_Groep_E.Models
{
    public class ChatUser
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public bool IsBlocked { get; set; }
    }
}
