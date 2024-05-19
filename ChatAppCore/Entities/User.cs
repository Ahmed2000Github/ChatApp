using Microsoft.AspNetCore.Identity;

namespace ChatAppCore.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public ICollection<UserConversation> UserConversations { get; set; }
    }
}
