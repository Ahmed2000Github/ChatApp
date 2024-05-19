

using ChatAppCore.Entities;

namespace ChatAppCore.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string ConversationId { get; set; }
        public bool IsReaded { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentDate { get; set; }
    }
}
