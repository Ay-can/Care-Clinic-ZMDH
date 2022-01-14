using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wdpr_Groep_E.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} heeft de groep gejoined {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} heeft de groep verlaten {groupName}.");
        }
    }
}
