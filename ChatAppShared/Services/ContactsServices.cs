using ChatAppCore.DTOs;
using ChatAppShared.Models;
using ChatAppShared.Services.Interfaces;
using System.Net.Http.Json;

namespace ChatAppShared.Services
{
    public class ContactsServices : IContactsServices
    {
        private readonly HttpClient httpClient;

        public ContactsServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            Contacts = null;
            LoadingStatus = LoadingStatus.LoadingInProgress;
        }

        public IEnumerable<ConversationContactDTO>? Contacts { get; set; }
        public IEnumerable<ConversationContactDTO>? FilteredContacts { get; set; }
        public LoadingStatus LoadingStatus { get; set; }

        public event Action? OnChange;



        public void Filter(string searchText)
        {
            Console.WriteLine(searchText);
            FilteredContacts = Contacts?.ToList().Where(u => ($"{u.FirstName} {u.LastName}").ToLower().Contains(searchText.ToLower())).ToList();
            OnChange?.Invoke();
        }
        public async Task Get(bool isFirstTime = false)
        {
            if (isFirstTime)
            {
                LoadingStatus = LoadingStatus.LoadingInProgress;
                OnChange?.Invoke();
            }

            try
            {
                Contacts = await httpClient.GetFromJsonAsync<List<ConversationContactDTO>>(AppConfig.CONTACTS_LIST_PATH);
                FilteredContacts = Contacts;
                if (isFirstTime)
                {
                    LoadingStatus = Contacts != null ? LoadingStatus.LoadingSucceed : LoadingStatus.LoadingFailed;
                }
            }
            catch (Exception)
            {
                if (isFirstTime)
                {
                    LoadingStatus = LoadingStatus.LoadingFailed;
                }
            }
            OnChange?.Invoke();
        }
        public async Task NotifyReaded(string conversationId)
        {
            await UpdateMessagesToReaded(conversationId);
        }
        public async Task Notify()
        {
            await Get();
        }
        private async Task UpdateMessagesToReaded(string conversationId)
        {
                var response = await httpClient.PutAsJsonAsync<string>(AppConfig.UPDATE_READED_MESSAGES_PATH, conversationId);

            if (response.IsSuccessStatusCode)
            {
                await Get();
            }

        }
    }
}
