using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace TekstilScada.Web.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var authToken = await _localStorage.GetItemAsync<string>("authToken");

                var identity = new ClaimsIdentity();
                _httpClient.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(authToken))
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                }

                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                // Geçersiz token durumunda tüm kimlik doğrulama bilgilerini temizle
                await _localStorage.RemoveItemAsync("authToken");
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(string authToken)
        {
            // Token'ı tırnak işaretlerinden arındırmadan kaydet
            await _localStorage.SetItemAsync("authToken", authToken);

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            // JWT token'ının üç parçalı formatını kontrol edin
            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                // Geçersiz format, boş bir liste döndür
                return claims;
            }

            try
            {
                // Payload'ı alın
                var payload = parts[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);

                // JSON'u bir sözlüğe dönüştürün
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                // Claim'leri oluşturun
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            catch (Exception)
            {
                // JSON çözme veya başka bir hata oluştuğunda boş liste döndür
                return new List<Claim>();
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
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