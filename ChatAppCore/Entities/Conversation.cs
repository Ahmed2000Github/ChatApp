namespace ChatAppCore.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IsHiddenFor { get; set; } = "";
        public string Name { get; set; }

        public ICollection<UserConversation> UserConversations { get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}
