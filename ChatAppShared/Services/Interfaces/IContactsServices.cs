using ChatAppCore.DTOs;
using ChatAppShared.Models;

namespace ChatAppShared.Services.Interfaces
{
    public interface IContactsServices
    {
        public IEnumerable<ConversationContactDTO>? Contacts { get; set; }
        public IEnumerable<ConversationContactDTO>? FilteredContacts { get; set; }
        public LoadingStatus LoadingStatus { get; set; }

        public event Action OnChange;

        public void Filter(string searchText);
        public Task Get();
        public Task Notify();

    }
}
