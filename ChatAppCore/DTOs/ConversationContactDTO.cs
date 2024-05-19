using System.Drawing;

namespace ChatAppCore.DTOs
{
    public class ConversationContactDTO
    {
        public string ConversationId { get; set; }
        public string ContactUserId { get; set; }
        public string LastSentMessage { get; set; }
        public string ContactAvatar { get; set; }
        public string FirstName { get; set; }
        public uint UnreadedCounter { get; set; } = 0;
        public string LastName { get; set; }
        public DateTime LastSentMessageDateTime { get; set; }
        public bool IsActive { get; set; } = false;
        
    }
}
