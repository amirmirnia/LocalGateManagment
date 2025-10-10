using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using static System.Net.WebRequestMethods;

namespace ServicesGateManagment.Client.Services.Core
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            var identity = new ClaimsIdentity();
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var claims = JwtParser.ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwtAuth");
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var claims = JwtParser.ParseClaimsFromJwt(token)
                .Select(c =>
                    c.Type.Contains("role") ? new Claim(ClaimTypes.Role, c.Value) :
                    c.Type.Contains("name") ? new Claim(ClaimTypes.Name, c.Value) : c);

            var identity = new ClaimsIdentity(claims, "jwtAuth");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }



        public void NotifyUserLogout()
        {
            //var identity = new ClaimsIdentity();
            //var user = new ClaimsPrincipal(identity);
            //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

            // 1. حذف توکن از LocalStorage
             _localStorage.RemoveItemAsync("authToken");

            // 2. پاک کردن هدر Authorization
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // 3. خالی کردن Claims و اعلام تغییر وضعیت
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }

    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
