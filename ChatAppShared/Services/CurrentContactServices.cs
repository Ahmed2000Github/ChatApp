using ChatAppCore.DTOs;
using ChatAppShared.Models;
using ChatAppShared.Services.Interfaces;
using System.Net.Http.Json;

namespace ChatAppShared.Services
{
    public class CurrentContactServices : ICurrentContactServices
    {
        public ConversationContactDTO? CurrentContact { get; set; }

        public event Action? OnChange;

        public void Notify(ConversationContactDTO currentContact)
        {
            CurrentContact = currentContact;
            OnChange?.Invoke();
        }
    }
}
