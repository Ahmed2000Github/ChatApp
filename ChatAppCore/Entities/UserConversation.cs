namespace ChatAppCore.Entities
{
    public class UserConversation
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid ConversationId { get; set; }
        public User User { get; set; }
        public Conversation Conversation { get; set; }
    }
}
