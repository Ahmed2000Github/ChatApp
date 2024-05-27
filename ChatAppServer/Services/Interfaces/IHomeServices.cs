using ChatAppCore.DTOs;
using ChatAppCore.Entities;

namespace ChatAppServer.Services.Interfaces
{
    public interface IHomeServices
    {
        public Task<IEnumerable<ConversationContactDTO>> GetAllUserContactConversations(string userId);
        public Task<IEnumerable<MessageDTO>> GetAllUserMessagesInConversation(string userId,string conversationId);
        public Task<MessageDTO> GetMessageById(string messageId);
        public Task<IEnumerable<UserToContactDTO>> GetAllUsersToContact(string userId);
        public Task<(MessageDTO okResult, string badRequestResult, string internalErrorResult)> AddMessageToConversation(MessageFormDTO message);
        public Task<(MessageDTO okResult, string badRequestResult, string internalErrorResult)> AddMessageFileToConversation(MessageFileFormDTO message);
        public Task<string> AddNewConversation(string userId, string recieverId);
        public Task<string> DeleteConversationById(string userId, string conversationId);
        public Task<bool> UpdateMessagesToReaded(string userId, string conversationId);
    }
}
