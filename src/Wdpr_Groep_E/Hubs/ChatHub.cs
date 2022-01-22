using Microsoft.AspNetCore.SignalR;

namespace Wdpr_Groep_E.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;
    }
}
