using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using TekstilScada.Models;
using System.Collections.Generic; // List<T> için
using System.Linq; // Split için
using System; // Convert için
using System.Text.Json; // JsonSerializer için

namespace TekstilScada.WebApp.Services
{
    // API'den dönen Token modelini varsayalım
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public User UserInfo { get; set; }
    }

    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _hasCheckedLocalStorage = false;

        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // --- ADIM 1: PRERENDERING GÜVENLİĞİ ---
        // Burası ASLA localStorage'a (JavaScript'e) dokunmamalı.
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_hasCheckedLocalStorage)
            {
                return new AuthenticationState(_currentUser);
            }

            // Prerendering sırasında "anonim" dön, JS hatası alma.
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }

        // --- ADIM 2: GERÇEK KONTROL (MainLayout'tan çağrılır) ---
        public async Task InitializeAuthenticationStateAsync()
        {
            if (_hasCheckedLocalStorage) return;

            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _currentUser = _anonymous;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                _currentUser = new ClaimsPrincipal(ParseClaimsFromJwt(token));
            }

            _hasCheckedLocalStorage = true;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // --- ADIM 3: GİRİŞ HATASININ ÇÖZÜMÜ (PascalCase JSON) ---
        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginPayload = new { Username = username, Password = password };

            // --- *** KODUNUZDAKİ EKSİK KISIM BURASI *** ---
            // API'nin PascalCase {"Username": ...} beklediğini söylüyoruz.
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };
            // ---------------------------------------------

            // API'ye 'serializerOptions' ile istek atıyoruz
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginPayload, serializerOptions);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

            // API 200 OK dönse bile, TOKEN gelmediyse (giriş hatalıysa) false dön
            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                return false; // "Kullanıcı adı yanlış" hatasını bu tetikler
            }

            // --- BAŞARILI GİRİŞ ---
            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            _currentUser = new ClaimsPrincipal(ParseClaimsFromJwt(loginResult.Token));
            _hasCheckedLocalStorage = true;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _currentUser = _anonymous;
            _hasCheckedLocalStorage = true;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // ... (ParseClaimsFromJwt metodu aynı kalır) ...
        private static ClaimsIdentity ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4)
            );

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Name, out object username);
            if (username != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, username.ToString()));
            }

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }
            }

            return new ClaimsIdentity(claims, "jwt");
        }
    }
}