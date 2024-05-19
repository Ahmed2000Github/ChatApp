namespace ChatAppCore.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsReaded { get; set; }
        public string IsHiddenFor { get; set; } = "";
        public MessageType Type { get; set; }
        public DateTime SentDate { get; set; }

        public string SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public User Sender { get; set; }
        public Conversation Conversation { get; set; }
    }
}
