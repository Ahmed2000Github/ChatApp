

using ChatAppCore.Entities;
using Microsoft.AspNetCore.Http;

namespace ChatAppCore.DTOs
{
    public class MessageFileFormDTO
    {
        public string SenderId { get; set; }
        public string ConversationId { get; set; } = "";
        public IFormFile FileContent { get; set; }
        public MessageType Type { get; set; }
    }
}
