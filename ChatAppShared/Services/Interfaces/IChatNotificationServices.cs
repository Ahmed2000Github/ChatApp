using ChatAppCore.DTOs;
using ChatAppShared.Models;

namespace ChatAppShared.Services.Interfaces
{
    public interface IChatNotificationServices
    {
        public List<MessageDTO>? Messages { get; set; }
        public IEnumerable<string>? OnlineUsers { get; set; }
        public LoadingStatus LoadingStatus { get; set; }
        public bool IsConnected { get; }

        public event Action? OnChange;

        public Task Get(string conversationId);
        public Task Start(string userId);
        public bool GetUserConnectivity(string userId);
        public Task DisposeAsync();
        public Task NotifyUser(MessageDTO message);
        public Task NotifyUserById(string userId);
        public Task Add(MessageDTO message);

    }
}
