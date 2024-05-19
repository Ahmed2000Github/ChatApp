

using ChatAppCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatAppInfrastructure
{

    public static class DataSeeder
    {
        public static void SeedData(DataContext context, UserManager<User> userManager)
        {
            if (!context.Users.Any())
            {
                // Seed users
                var _users = new List<User>
                    {
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Avatar = "uploads/avatars/1.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Avatar = "uploads/avatars/2.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user3", Email = "user3@example.com", FirstName = "Alice", LastName = "Johnson", Avatar = "uploads/avatars/3.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user4", Email = "user4@example.com", FirstName = "Bob", LastName = "Brown", Avatar = "uploads/avatars/4.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user5", Email = "user5@example.com", FirstName = "Emma", LastName = "Wilson", Avatar = "uploads/avatars/5.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user6", Email = "user6@example.com", FirstName = "Michael", LastName = "Taylor", Avatar = "uploads/avatars/6.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user7", Email = "user7@example.com", FirstName = "Olivia", LastName = "Anderson", Avatar = "uploads/avatars/7.png" },
                        new User { Id = Guid.NewGuid().ToString(), UserName = "user8", Email = "user8@example.com", FirstName = "William", LastName = "Martinez", Avatar = "uploads/avatars/8.png" }
                    };

                foreach (var user in _users)
                {
                    var result = userManager.CreateAsync(user, "Password123").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user: {user.UserName}");
                    }
                }
            }
            //var users = context.Users.Take(2).ToList();
            //if (!context.Conversations.Any())
            //{
            //    // Seed conversation
            //    var conversation = new Conversation
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Sample Conversation",
            //        CreatedDate = DateTime.Now
            //    };

            //    context.Conversations.Add(conversation);
            //    context.SaveChanges();

            //    // Add two users to the conversation
              
            //    foreach (var user in users)
            //    {
            //        var userConversation = new UserConversation
            //        {
            //            Id = Guid.NewGuid(),
            //            UserId = user.Id,
            //            ConversationId = conversation.Id
            //        };

            //        context.UserConversations.Add(userConversation);
            //    }

            //    context.SaveChanges();
            //}

            //if (!context.Messages.Any())
            //{
            //    // Seed messages
            //    var conversationId = context.Conversations.FirstOrDefault().Id;
            //    var messageContents = new string[]
            //        {
            //             "Hello, how are you?",
            //             "I'm doing great, thanks!",
            //             "What are you up to today?",
            //             "Just working on some coding projects.",
            //             "Sounds interesting! What language are you using?",
            //             "Mainly C# and JavaScript.",
            //             "Cool! I'm learning Python myself.",
            //             "Python is great for data science and AI.",
            //             "Yeah, that's why I'm learning it.",
            //             "Let me know if you need any help!"
            //        };


            //    var random = new Random();
            //    for (int i = 0; i < 10; i++)
            //    {
            //        var senderId = users[random.Next(users.Count)].Id;
            //        var message = new Message
            //        {
            //            Id = Guid.NewGuid(),
            //            Content = messageContents[i],
            //            SentDate = DateTime.Now.AddMinutes(i),
            //            SenderId = senderId,
            //            ConversationId = conversationId
            //        };

            //        context.Messages.Add(message);
            //    }

            //    context.SaveChanges();
            //}
            //if (!context.Contacts.Any())
            //{
            //    // Seed contacts
            //    var users = context.Users.ToList();

            //    // Each user adds all other users as contacts
            //    foreach (var user in users)
            //    {
            //        foreach (var contactUser in users.Where(u => u.Id != user.Id))
            //        {
            //            var contact = new Contact
            //            {
            //                Id = Guid.NewGuid(),
            //                UserId = user.Id,
            //                ContactUserId = contactUser.Id,
            //                AddedDate = DateTime.Now
            //            };

            //            context.Contacts.Add(contact);
            //        }
            //    }

            //    context.SaveChanges();
            //}
        }
    }

}
