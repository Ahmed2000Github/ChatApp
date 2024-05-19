namespace ChatAppCore.DTOs
{
    public class ConversationDTO
    {
        public Guid Id { get; set; }
        public Guid SUserId { get; set; }
        public Guid RUserId { get; set; }
    }
}
