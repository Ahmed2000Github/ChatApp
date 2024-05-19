

using ChatAppCore.Entities;

namespace ChatAppCore.DTOs
{
    public class MessageFormDTO
    {
        public string SenderId { get; set; }
        public string ConversationId { get; set; } = "";
        public string Content { get; set; }
        public MessageType Type { get; set; }
    }
}
