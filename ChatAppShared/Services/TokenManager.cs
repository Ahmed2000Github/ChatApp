using Blazored.LocalStorage;
using ChatAppCore.DTOs;
using ChatAppShared.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ChatAppShared.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public TokenManager(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }
        public async Task<bool> CheckTokenExperatibility()
        {
            await Console.Out.WriteLineAsync("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            var accessTokenExpireIn = await _localStorage.GetItemAsync<DateTime>("AccessTokenExpireIn");
            var refreshTokenExpireIn = await _localStorage.GetItemAsync<DateTime>("RefreshTokenExpireIn");
            if (accessTokenExpireIn.CompareTo(DateTime.Now)>0)
            {
                await Console.Out.WriteLineAsync("Token valid 0000000000000000000000000");
                Console.WriteLine("Starting chat connection ...");
                return true;
            }
            if (refreshTokenExpireIn.CompareTo(DateTime.Now) > 0)
            {
            var refreshToken = await _localStorage.GetItemAsync<string>("RefreshToken");
                var response = await _httpClient.PostAsJsonAsync(AppConfig.REFRESH_TOKEN, refreshToken);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    await Console.Out.WriteLineAsync("refresh response 0000000000000000000000000");
                    var tokenData = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
                    await StoreTokens(tokenData);
                    _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
                    return false;
                }
                await Console.Out.WriteLineAsync("refresh failed 0000000000000000000000000");
            }
            await Console.Out.WriteLineAsync("empty storage 0000000000000000000000000");
            await EmptyLocalStorage();
            await Console.Out.WriteLineAsync("to login 0000000000000000000000000");
            _navigationManager.NavigateTo("/login");
            return false;

        }

        public async Task StoreTokens(LoginResponseDTO data)
        {
            await _localStorage.SetItemAsync("AccessToken", data.AccessToken);
            await _localStorage.SetItemAsync("RefreshToken", data.RefreshToken);
            await _localStorage.SetItemAsync("AccessTokenExpireIn", data.AccessTokenExpireIn);
            await _localStorage.SetItemAsync("RefreshTokenExpireIn", data.RefreshTokenExpireIn);
        }

        public async Task EmptyLocalStorage()
        {
            await _localStorage.RemoveItemAsync("AccessToken");
            await _localStorage.RemoveItemAsync("RefreshToken");
            await _localStorage.RemoveItemAsync("AccessTokenExpireIn");
            await _localStorage.RemoveItemAsync("RefreshTokenExpireIn");
        }
    }
}
