﻿using Blazored.LocalStorage;
using ChatAppCore.DTOs;
using ChatAppCore.Entities;
using ChatAppShared.Models;
using ChatAppShared.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ChatAppShared.Services
{
    public class ChatNotificationServices : IChatNotificationServices
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;
        private readonly IContactsServices? _contactsServices;
        private readonly ICurrentContactServices? _currentContactServices;
        private HubConnection? _hubConnection;

        public ChatNotificationServices(HttpClient httpClient, IJSRuntime jsRuntime, ILocalStorageService localStorage, IContactsServices? contactsServices, ICurrentContactServices? currentContactServices)
        {
            this.httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
            Messages = new List<MessageDTO>();
            LoadingStatus = LoadingStatus.LoadingInProgress;
            _contactsServices = contactsServices;
            _currentContactServices = currentContactServices;
        }

        public List<MessageDTO>? Messages { get; set; }
        public IEnumerable<string>? OnlineUsers { get; set; }
        public LoadingStatus LoadingStatus { get; set; }
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public event Action? OnChange;


        public async Task Start(string userId)
        {
            await CreateConnection();


            _hubConnection.On<MessageDTO>("ReceiveNotification", async (message) =>
            {
                if (message is not null && _currentContactServices.CurrentContact?.ConversationId == message.ConversationId)
                {
                    Messages.Add(message);
                    OnChange?.Invoke();
                    _contactsServices.NotifyReaded(message.ConversationId ?? "");
                    await _jsRuntime.InvokeAsync<string>("scrollToBottom", "scrollableDiv");
                }
                else
                {
                 _contactsServices.Notify();
                }

            });

            _hubConnection.On<IEnumerable<string>>("OnUsersListChange", (users) =>
            {
                OnlineUsers = users;
                OnChange?.Invoke();
            });


            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection?.StartAsync();
            }

            LoadingStatus = LoadingStatus.LoadingSucceed;
            OnChange?.Invoke();
            Console.WriteLine("Connected");
        }

        public bool GetUserConnectivity(string userId)
        {
            if (OnlineUsers is null)
            {

                return false;
            }
            return OnlineUsers.Contains(userId);
        }
        public async Task DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
                await Console.Out.WriteLineAsync("dispose ......");
            }
        }

        public async Task Add(MessageDTO message)
        {
            await _contactsServices.Notify();
            Messages.Add(message);
            OnChange?.Invoke();
        }
        public async Task NotifyUser(MessageDTO message)
        {
            var userId = _currentContactServices.CurrentContact.ContactUserId ?? "";
            var conversationId = _currentContactServices.CurrentContact.ConversationId ?? "";
            if (_hubConnection is not null && userId != "")
            {
                await _hubConnection.SendAsync("NotifyUser", userId,message);
                OnChange?.Invoke();
            }
        }
        public async Task NotifyUserById(string userId)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("NotifyUser", userId, null);
                OnChange?.Invoke();
            }
        }

        public async Task Get(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                await Console.Out.WriteLineAsync("null id");
                return;
            }
            LoadingStatus = LoadingStatus.LoadingInProgress;
            OnChange?.Invoke();
            try
            {
                Messages = await httpClient.GetFromJsonAsync<List<MessageDTO>>($"{AppConfig.MESSAGES_LIST_PATH}/{conversationId}");

                LoadingStatus = Messages != null ? LoadingStatus.LoadingSucceed : LoadingStatus.LoadingFailed;
            }
            catch (Exception)
            {
                LoadingStatus = LoadingStatus.LoadingFailed;

            }
            OnChange?.Invoke();
        }
        private async Task CreateConnection()
        {
            _hubConnection = new HubConnectionBuilder()
               .WithUrl(new Uri($"{AppConfig.BASE_URL}{AppConfig.CHATHUB_PATH}"), options =>
               {
                   options.AccessTokenProvider = async () =>
                   {
                       return (await _localStorage.GetItemAsStringAsync("AccessToken")).Replace("\"", "");
                   };
               }
               )
               .WithAutomaticReconnect()
               .Build();
        }

    }
}
