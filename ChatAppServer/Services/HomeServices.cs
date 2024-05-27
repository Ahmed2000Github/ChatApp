using AutoMapper;
using ChatAppCore.DTOs;
using ChatAppCore.Entities;
using ChatAppCore.Interfaces;
using ChatAppServer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

namespace ChatAppServer.Services
{
    public class HomeServices : IHomeServices
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork<Message> _messages;
        private readonly IUnitOfWork<Conversation> _conversations;
        private readonly IUnitOfWork<UserConversation> _userConversations;
        private readonly IFileManagerServices _fileManagerServices;
        private readonly IMapper _mapper;
        private const string MESSAGE_SEPARATOR = "@@##&&!!";

        public HomeServices(IConfiguration configuration,
            UserManager<User> userManager,
            IMapper mapper,
            IUnitOfWork<Message> messages,
            IUnitOfWork<Conversation> conversations,
            IUnitOfWork<UserConversation> userConversations,
            IFileManagerServices fileManagerServices)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _messages = messages;
            _conversations = conversations;
            _userConversations = userConversations;
            _fileManagerServices = fileManagerServices;
        }

        // This method is for retrive a list of all user's contacts conversaions 
        public async Task<IEnumerable<ConversationContactDTO>> GetAllUserContactConversations(string userId)
        {
            var conversations = _userConversations.entity
                .Where(x => x.UserId == userId).ToList();

            var userConversations = _userConversations.entity
                .Where(x => x.UserId != userId && conversations.Select(x => x.ConversationId).Contains(x.ConversationId))
                .Join(conversations, uc => uc.ConversationId, uc => uc.ConversationId,
                 (uc1, uc2) => new
                 {
                     ConversationId = uc1.ConversationId,
                     UserId = uc2.UserId,
                     ContactId = uc1.UserId
                 }).ToList();
            var contactsDTO = userConversations.Select(uc =>
            {
                var contact = _userManager.Users.Where(u => u.Id == uc.ContactId).FirstOrDefault();
                var conversation = _conversations.entity.GetById(uc.ConversationId);
                if (conversation.IsHiddenFor == userId)
                {
                    return null;
                }
                var message = _messages.entity
                   .GetAll()
                   .Where(x => x.ConversationId == uc.ConversationId)
                   .OrderByDescending(m => m.SentDate)
                   .FirstOrDefault();
                if (message == null || message.IsHiddenFor == userId)
                {
                    message = new Message
                    {
                        ConversationId = uc.ConversationId,
                        SentDate = DateTime.MinValue,
                        Content = ""
                    };
                }
                Expression<Func<Message, bool>> selectionCdt = (Message x) => x.ConversationId == uc.ConversationId && !x.IsReaded && x.SenderId != userId && x.IsHiddenFor != userId;
                var counter = (uint)_messages.entity.Where(selectionCdt).Count();
                return new ConversationContactDTO
                {
                    ContactUserId = contact.Id,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    ContactAvatar = contact.Avatar,
                    LastSentMessage = message.Content,
                    LastSentMessageDateTime = message.SentDate,
                    ConversationId = uc.ConversationId.ToString(),
                    UnreadedCounter = counter
                };
            })
                .Where(x => x is not null);
            return contactsDTO;
        }

        // This method is for retrive a message by its Id
        public async Task<MessageDTO> GetMessageById(string messageId)
        {
            var message = _messages.entity.Where(x => x.Id.ToString() == messageId).FirstOrDefault();
            return  _mapper.Map<MessageDTO>(message);
        }

        // This method is for retrive a list of all conversation's messages 
        public async Task<IEnumerable<MessageDTO>> GetAllUserMessagesInConversation(string userId, string conversationId)
        {
            var conversation = _conversations.entity.GetById(new Guid(conversationId));
            if (conversation is not null)
            {
                var messages = _messages.entity.Where(x => x.ConversationId.ToString() == conversationId && !x.IsReaded && x.SenderId != userId);
                foreach (var message in messages)
                {
                        message.IsReaded = true;
                }
                _messages.entity.UpdateRange(messages.ToArray());
                _messages.save();
                messages = _messages.entity.Where(x => x.ConversationId.ToString() == conversationId && x.IsHiddenFor != userId).OrderBy(m => m.SentDate);
                return messages.Select(message => _mapper.Map<MessageDTO>(message));
            }
            else
            {
                return null;
            }

        }

        // this method retrive a list of all user to contacts
        public async Task<IEnumerable<UserToContactDTO>> GetAllUsersToContact(string userId)
        {
            var allUsersConversations = _userConversations.entity.GetAll();
            var conversationsIds = allUsersConversations
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ConversationId).ToList();
            var filteredConversationIds = conversationsIds.Where(conversationId =>
            {
                var conversation = _conversations.entity.GetById(conversationId);
                return conversation.IsHiddenFor != userId;
            }).ToList();
            var usersConversationIds = allUsersConversations
                .Where(uc => filteredConversationIds.Contains(uc.ConversationId) && uc.UserId != userId)
                .Select(uc => uc.UserId);
            var users = _userManager.Users.Where(u => !usersConversationIds.Contains(u.Id) && u.Id != userId);

            var list = new List<UserToContactDTO>();
            foreach (var user in users)
            {
                list.Add(
                    new UserToContactDTO()
                    {
                        Id = user.Id,
                        Avatar = user.Avatar,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                    });
            }

            return list;
        }
        // This method is for adding a new message to an existing conversation
        public async Task<(MessageDTO okResult, string badRequestResult, string internalErrorResult)> AddMessageToConversation(MessageFormDTO message)
        {
            var conversation = _conversations.entity.GetById(new Guid(message.ConversationId));
            if (!string.IsNullOrEmpty(conversation.IsHiddenFor))
            {
                conversation.IsHiddenFor = "";
                _conversations.entity.Update(conversation);
                _conversations.save();
            }
            try
            {
                // insert new message
                var newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    ConversationId = new Guid(message.ConversationId),
                    Content = message.Content,
                    Type = MessageType.Text,
                    SenderId = message.SenderId,
                    SentDate = DateTime.Now,
                    IsReaded = false
                };
                _messages.entity.Add(newMessage);
                _messages.save();
                var messageDTO = new MessageDTO()
                {
                    Id = newMessage.Id.ToString(),
                    Content = newMessage.Content,
                    SentDate = newMessage.SentDate,
                    ConversationId = newMessage.ConversationId.ToString(),
                    SenderId = newMessage.SenderId,
                    Type = newMessage.Type,
                    IsReaded = newMessage.IsReaded
                };
                return (messageDTO, null, null);
            }
            catch (Exception e)
            {
                return (null, null, e.Message);
            }

        }

        // This method is for adding a new message file to an existing conversation
        public async Task<(MessageDTO okResult, string badRequestResult, string internalErrorResult)> AddMessageFileToConversation(MessageFileFormDTO message)
        {
            var conversation = _conversations.entity.GetById(new Guid(message.ConversationId));
            if (!string.IsNullOrEmpty(conversation.IsHiddenFor))
            {
                conversation.IsHiddenFor = "";
                _conversations.entity.Update(conversation);
                _conversations.save();
            }
            try
            {
                if (message.FileContent == null || message.FileContent.Length == 0)
                    return (null, "File is empty.", null);
                var relativePath = await _fileManagerServices.UploadFile(message.FileContent);
                // insert new message
                var newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    ConversationId = new Guid(message.ConversationId),
                    Content = $"{message.FileContent.FileName}{MESSAGE_SEPARATOR}{relativePath}",
                    Type = message.Type,
                    SenderId = message.SenderId,
                    SentDate = DateTime.Now,
                    IsReaded = false
                };
                _messages.entity.Add(newMessage);
                _messages.save();
                var messageDTO = new MessageDTO()
                {
                    Id = newMessage.Id.ToString(),
                    Content = newMessage.Content,
                    SentDate = newMessage.SentDate,
                    ConversationId = newMessage.ConversationId.ToString(),
                    SenderId = newMessage.SenderId,
                    Type = newMessage.Type,
                    IsReaded = newMessage.IsReaded
                };
                return (messageDTO, null, null);
            }
            catch (Exception ex)
            {
                return (null, null, $"Internal server error: {ex.Message}");
            }
        }
        public async Task<bool> UpdateMessagesToReaded(string userId,string conversationId)
        {
            var messages = _messages.entity.Where(x => x.ConversationId.ToString() == conversationId);
            foreach (var message in messages)
            {
                if (!message.IsReaded && message.SenderId != userId)
                {
                    message.IsReaded = true;
                }
            }
            _messages.entity.UpdateRange(messages.ToArray());
            _messages.save();
            return true;
        }

        // This method is for adding a new conversation
        public async Task<string> AddNewConversation(string userId, string recieverId)
        {
            var userConversations = _userConversations.entity.Where(uc => uc.UserId == recieverId).ToList();
            var existConversationId = _userConversations.entity.Where(uc => uc.UserId == userId)
                .Join(userConversations, uc => uc.ConversationId, uc => uc.ConversationId,
                 (uc1, uc2) => new
                 {
                     ConversationId = uc1.ConversationId
                 }).FirstOrDefault()?.ConversationId;

            var conversation = _conversations.entity.GetById(existConversationId ?? Guid.Empty);
            if (conversation is not null)
            {
                if (!string.IsNullOrEmpty(conversation.IsHiddenFor))
                {
                    conversation.IsHiddenFor = "";
                    _conversations.entity.Update(conversation);
                    _conversations.save();
                }
                return "Conversation was created successfuly.";
            }

            var conversationId = Guid.NewGuid();
            // insert new conversation
            var newConversation = new Conversation()
            {
                Id = conversationId,
                Name = "default",
                CreatedDate = DateTime.Now,

            };
            _conversations.entity.Add(newConversation);
            _conversations.save();

            // add two new usercoversations
            var firstUserConversation = new UserConversation()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ConversationId = conversationId
            };
            var secondUserConversation = new UserConversation()
            {
                Id = Guid.NewGuid(),
                UserId = recieverId,
                ConversationId = conversationId
            };
            _userConversations.entity.Add(firstUserConversation);
            _userConversations.entity.Add(secondUserConversation);
            _userConversations.save();

            return "Conversation was created successfuly.";
        }

        // This method is for deleteing a conversation by its id
        public async Task<string> DeleteConversationById(string userId, string conversationId)
        {

            var conversation = _conversations.entity.GetById(new Guid(conversationId));
            var otherUserId = _userConversations.entity.Where(u => u.ConversationId.ToString() == conversationId && u.UserId != userId).FirstOrDefault().UserId;
            if (conversation is not null && conversation.IsHiddenFor == otherUserId)
            {
                // delete messages of conversation
                var messages = _messages.entity.Where(m => m.ConversationId.ToString() == conversationId);
                foreach (var message in messages)
                {
                    if (message.Type != MessageType.Text)
                    {
                        _fileManagerServices.DeleteFile(message.Content.Split(MESSAGE_SEPARATOR).Last());
                    }
                }
                _messages.entity.DeleteRange(messages.ToArray());
                _messages.save();

                // delete userconversations of conversation
                var userConversations = _userConversations.entity.Where(m => m.ConversationId.ToString() == conversationId);
                _userConversations.entity.DeleteRange(userConversations.ToArray());
                _userConversations.save();
                // delete conversation
                _conversations.entity.Delete(new Guid(conversationId));
                _conversations.save();
                return "Conversation was deleted successfuly.";
            }
            else if (conversation is not null)
            {
                // delete messages of conversation
                var messages = _messages.entity.Where(m => m.ConversationId.ToString() == conversationId);
                foreach (var message in messages)
                {
                    if (string.IsNullOrEmpty(message.IsHiddenFor))
                    {
                        message.IsHiddenFor = userId;
                        _messages.entity.Update(message);
                    }
                    else
                    {
                        if (message.Type != MessageType.Text)
                        {
                            _fileManagerServices.DeleteFile(message.Content.Split(MESSAGE_SEPARATOR).Last());
                        }
                        _messages.entity.Delete(message.Id);
                    }
                }
                _messages.save();



                // delete conversation
                conversation.IsHiddenFor = userId;
                _conversations.entity.Update(conversation);
                _conversations.save();
                return "Conversation was deleted successfuly.";
            }
            else
            {
                return "Conversation not exist";
            }
        }


    }
}
