using Blazored.LocalStorage;
using ChatAppShared.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ChatAppShared.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ITokenManager _tokenManager;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, ITokenManager tokenManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _tokenManager = tokenManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsStringAsync("AccessToken");
            if (string.IsNullOrWhiteSpace(token))
                return await NotifyUserLogout();
            return await NotifyUserLogin(token);
        }


        public async Task<AuthenticationState> NotifyUserLogout()
        {
            await _tokenManager.EmptyLocalStorage();
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
            return _anonymous;
        }
        public async Task<AuthenticationState> NotifyUserLogin(string token)
        {
            var identity = new ClaimsIdentity();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token.Replace("\"", "")))
            {
                identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
    }
}
