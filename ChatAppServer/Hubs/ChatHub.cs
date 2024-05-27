using ChatAppCore.DTOs;
using ChatAppCore.Entities;
using ChatAppServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatAppServer.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        private static Dictionary<string, List<string>> userConnections = new Dictionary<string, List<string>>();


        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!userConnections.ContainsKey(userId))
            {
                userConnections[userId] = new List<string>();
               
            }
            await Clients.All.SendAsync("OnUsersListChange", userConnections.Keys);
            userConnections[userId].Add(Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user ID

            if (userConnections.ContainsKey(userId))
            {
                userConnections[userId].Remove(Context.ConnectionId);
                if (userConnections[userId].Count == 0)
                {
                    userConnections.Remove(userId);
                    await Clients.All.SendAsync("OnUsersListChange", userConnections.Keys);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotifyUser(string userId, MessageDTO message)
        {
            if (userConnections.ContainsKey(userId))
            {
                foreach (var connectionId in userConnections[userId])
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
                }
            }
        }


    }

}
