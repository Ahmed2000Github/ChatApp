using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppShared
{
    public static class AppConfig
    {
        public const long MAX_FILE_SIZE = 10_000_000L;
        public const string MESSAGE_SEPARATOR = "@@##&&!!";
        public const string BASE_URL = "https://localhost:7134/";
        public const string CHATHUB_PATH = "chathub";
        public const string LOGIN_PATH = "api/User/Login";
        public const string SIGNUP_PATH = "api/User/Register";
        public const string REFRESH_TOKEN = "api/User/Refresh";
        public const string CONTACTS_LIST_PATH = "api/Home/GetContactConversations";
        public const string USERS_LIST_PATH = "api/Home/GetUsersToContact";
        public const string MESSAGES_LIST_PATH = "/api/Home/GetMessages";
        public const string ADD_MESSAGE_PATH = "api/Home/AddMessage";
        public const string UPDATE_READED_MESSAGE_PATH = "api/Home/UpdateMessage";
        public const string ADD_CONVERSATION_PATH = "api/Home/AddConversation";
        public const string DELETE_CONVERSATION_PATH = "api/Home/DeleteConversationById";
        public const string UPLOAD_FILE_PATH = "api/Home/UploadFile";
    }
}
